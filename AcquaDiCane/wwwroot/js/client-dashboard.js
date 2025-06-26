document.addEventListener('DOMContentLoaded', function () {
    const sidebarButtons = document.querySelectorAll('.sidebar-button'); // Ahora son botones
    const dashboardSections = document.querySelectorAll('.dashboard-section');
    const petsOverviewContainer = document.getElementById('petsOverviewContainer');
    const noPetsMessage = document.getElementById('noPetsMessage');
    const btnPeluqueria = document.getElementById('btnPeluqueria'); // El botón Peluquería
    const logoutButton = document.getElementById('logoutButton');

    // Forms & Messages
    const addPetForm = document.getElementById('addPetForm');
    const addPetMessages = document.getElementById('addPetMessages');
    const scheduleAppointmentForm = document.getElementById('scheduleAppointmentForm');
    const scheduleAppointmentMessages = document.getElementById('scheduleAppointmentMessages');
    const noPetsForAppointmentMessage = document.getElementById('noPetsForAppointmentMessage');
    const noServicesOrGroomersMessage = document.getElementById('noServicesOrGroomersMessage');
    const appointmentPetSelect = document.getElementById('appointmentPet');
    const appointmentServiceSelect = document.getElementById('appointmentService');
    const appointmentGroomerSelect = document.getElementById('appointmentGroomer'); // Nuevo select de groomer
    const appointmentPriceInput = document.getElementById('appointmentPrice');
    const appointmentsListContainer = document.getElementById('appointmentsListContainer');

    // Profile elements (mantener si planeas dar acceso al perfil)
    const profileName = document.getElementById('profileName');
    const profileLastName = document.getElementById('profileLastName');
    const profileEmail = document.getElementById('profileEmail');
    const profilePhoneNumber = document.getElementById('profilePhoneNumber');
    const profileBirthDate = document.getElementById('profileBirthDate');
    const editProfileBtn = document.getElementById('editProfileBtn');
    const editProfileFormContainer = document.getElementById('editProfileFormContainer');
    const editProfileForm = document.getElementById('editProfileForm');
    const cancelEditProfile = document.getElementById('cancelEditProfile');
    const changePasswordForm = document.getElementById('changePasswordForm');
    const editProfileMessages = document.getElementById('editProfileMessages');
    const changePasswordMessages = document.getElementById('changePasswordMessages');

    let availableServices = [];
    let availableGroomers = [];
    let currentUserPets = []; // Para almacenar las mascotas del usuario


    // --- Utility Functions ---

    /**
     * Muestra un mensaje en un contenedor de formulario.
     * @param {HTMLElement} container El contenedor del mensaje.
     * @param {string} message El texto del mensaje.
     * @param {boolean} isSuccess Si es true, el mensaje es de éxito; de lo contrario, de error.
     */
    function showFormMessage(container, message, isSuccess) {
        container.innerHTML = message;
        container.className = 'form-messages ' + (isSuccess ? 'success' : 'error');
        container.style.display = 'block';
    }

    /**
     * Oculta un mensaje de formulario.
     * @param {HTMLElement} container El contenedor del mensaje.
     */
    function hideFormMessage(container) {
        container.style.display = 'none';
        container.innerHTML = '';
        container.className = 'form-messages';
    }

    /**
     * Formatea una fecha de YYYY-MM-DD a DD/MM/YYYY.
     * @param {string} dateString La fecha en formato YYYY-MM-DD.
     * @returns {string} La fecha formateada.
     */
    function formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('es-AR', { year: 'numeric', month: '2-digit', day: '2-digit' });
    }

    /**
     * Formatea una hora de HH:MM:SS a HH:MM.
     * @param {string} timeString La hora en formato HH:MM:SS.
     * @returns {string} La hora formateada.
     */
    function formatTime(timeString) {
        if (!timeString) return '';
        const parts = timeString.split(':');
        return `${parts[0]}:${parts[1]}`;
    }

    // --- Navigation Logic ---
    function showSection(sectionId) {
        dashboardSections.forEach(section => {
            section.classList.remove('active');
        });
        document.getElementById(sectionId).classList.add('active');

        sidebarButtons.forEach(button => {
            button.classList.remove('active');
            if (button.dataset.section === sectionId.replace('-section', '')) {
                button.classList.add('active');
            }
        });

        // Lógica específica al cambiar de sección
        if (sectionId === 'add-pet-section') {
            addPetForm.reset();
            hideFormMessage(addPetMessages);
            document.getElementById('noBreedCheckbox').checked = false; // Reset checkbox
            document.getElementById('petBreed').removeAttribute('readonly');
            document.getElementById('petBreed').value = '';
        } else if (sectionId === 'schedule-appointment-section') {
            scheduleAppointmentForm.reset();
            hideFormMessage(scheduleAppointmentMessages);
            appointmentPriceInput.value = '';
            loadPetsForAppointment();
            loadAvailableServices();
            loadAvailableGroomers();
            loadAppointments(); // Cargar la lista de turnos del cliente
        } else if (sectionId === 'overview-section') {
            loadPets(); // Recargar mascotas para el overview
            loadAppointments(); // Recargar turnos para el overview
        } else if (sectionId === 'profile-section') {
            loadProfile();
            editProfileFormContainer.style.display = 'none';
            hideFormMessage(editProfileMessages);
            hideFormMessage(changePasswordMessages);
            editProfileForm.reset();
            changePasswordForm.reset();
        }
    }

    // Event listeners for sidebar buttons
    sidebarButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            const section = this.dataset.section + '-section';
            showSection(section);
        });
    });

    // Handle internal button clicks to switch sections (e.g., "Registrar mi primera mascota")
    document.querySelectorAll('[data-section="add-pet-from-empty"]').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            showSection('add-pet-section');
        });
    });

    document.querySelectorAll('[data-section="cancel-add-pet"]').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            showSection('overview-section');
        });
    });


    // --- API Calls & Data Rendering ---

    // Function to get current user ID
    async function getCurrentUserId() {
        try {
            const response = await fetch('/api/accountapi/current-user-id'); // Ajusta la ruta si tu API Controller se llama diferente
            if (!response.ok) {
                if (response.status === 401) { // Unauthorized
                    window.location.href = '/Account/Login'; // Redirigir al login si no está autenticado
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const userId = await response.json();
            return userId;
        } catch (error) {
            console.error('Error al obtener el ID del usuario:', error);
            // Considerar redirigir al login aquí también en caso de error de red
            return null;
        }
    }

    // Load Pets for Overview
    async function loadPets() {
        petsOverviewContainer.innerHTML = ''; // Clear previous content
        noPetsMessage.style.display = 'none'; // Hide "no pets" message by default
        btnPeluqueria.disabled = true; // Disable "Peluqueria" button by default

        try {
            const userId = await getCurrentUserId();
            if (!userId) {
                // Si el usuario no está identificado, el getCurrentUserId ya debería haber redirigido
                return;
            }

            const response = await fetch(`/api/clientapi/pets?ownerId=${userId}`); // Ajusta la ruta
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            currentUserPets = await response.json(); // Store pets globally

            if (currentUserPets.length === 0) {
                noPetsMessage.style.display = 'block'; // Show "no pets" message
                petsOverviewContainer.style.display = 'none'; // Hide pets grid
            } else {
                noPetsMessage.style.display = 'none'; // Hide "no pets" message
                petsOverviewContainer.style.display = 'grid'; // Show pets grid
                btnPeluqueria.disabled = false; // Enable "Peluqueria" button

                currentUserPets.forEach(pet => {
                    const petCard = document.createElement('div');
                    petCard.classList.add('pet-card');
                    petCard.innerHTML = `
                        <div class="pet-card-header">${pet.name}</div>
                        <div class="pet-card-body">
                            <img src="${pet.profilePicUrl && !pet.profilePicUrl.startsWith('~') ? pet.profilePicUrl : '/img/default_pet.png'}" alt="Foto de ${pet.name}">
                            <p><strong>Raza:</strong> ${pet.breed || (pet.noBreed ? 'Mestizo' : 'N/A')}</p>
                            <p><strong>Tamaño:</strong> ${pet.size}</p>
                            <p><strong>Sexo:</strong> ${pet.sex}</p>
                            <p><strong>Peso:</strong> ${pet.weight} kg</p>
                            <p><strong>Nacimiento:</strong> ${formatDate(pet.birthDate)}</p>
                            <p><strong>Castrado:</strong> ${pet.castrated ? 'Sí' : 'No'}</p>
                            <p><strong>Alergias:</strong> ${pet.allergies ? 'Sí' : 'No'}</p>
                            <p><strong>Próximo Turno:</strong> <span id="nextAppointment-${pet.id}">Cargando...</span></p>
                        </div>
                        <div class="pet-card-actions">
                            <button onclick="editPet(${pet.id})">Editar</button>
                            <button onclick="deletePet(${pet.id})">Eliminar</button>
                        </div>
                    `;
                    petsOverviewContainer.appendChild(petCard);
                    loadNextAppointmentForPet(pet.id); // Load next appointment for each pet
                });
            }
        } catch (error) {
            console.error('Error al cargar las mascotas:', error);
            petsOverviewContainer.innerHTML = '<p class="error-message">Error al cargar las mascotas. Intente de nuevo más tarde.</p>';
            noPetsMessage.style.display = 'none'; // Ensure this is hidden if there's an error
            btnPeluqueria.disabled = true;
        }
    }

    // Load next appointment for a specific pet
    async function loadNextAppointmentForPet(petId) {
        const nextAppointmentSpan = document.getElementById(`nextAppointment-${petId}`);
        if (!nextAppointmentSpan) return;

        try {
            const response = await fetch(`/api/clientapi/appointments/next?petId=${petId}`);
            if (!response.ok) {
                // If no next appointment, it's fine, just display "No tiene turnos reservados"
                nextAppointmentSpan.textContent = 'No tiene turnos reservados';
                return;
            }
            const appointment = await response.json();
            if (appointment) {
                nextAppointmentSpan.innerHTML = `${formatDate(appointment.date)} - ${formatTime(appointment.time)}<br>Servicio: ${appointment.service}`;
            } else {
                nextAppointmentSpan.textContent = 'No tiene turnos reservados';
            }
        } catch (error) {
            console.error(`Error al cargar el próximo turno para la mascota ${petId}:`, error);
            nextAppointmentSpan.textContent = 'Error al cargar el turno.';
        }
    }


    // Load Appointments for "Mis Próximos Turnos" in the Peluqueria section
    async function loadAppointments() {
        appointmentsListContainer.innerHTML = '<p>Cargando turnos...</p>';
        try {
            const userId = await getCurrentUserId();
            if (!userId) {
                appointmentsListContainer.innerHTML = '<p class="error-message">No se pudo cargar los turnos. Usuario no identificado.</p>';
                return;
            }

            const response = await fetch(`/api/clientapi/appointments?clientId=${userId}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const appointments = await response.json();

            if (appointments.length === 0) {
                appointmentsListContainer.innerHTML = '<p>No tienes turnos agendados.</p>';
                return;
            }

            appointmentsListContainer.innerHTML = ''; // Clear loading message

            appointments.forEach(appt => {
                const appointmentCard = document.createElement('div');
                appointmentCard.classList.add('appointment-card');
                appointmentCard.innerHTML = `
                    <div class="appointment-info">
                        <h3>${appt.serviceName} para ${appt.petName}</h3>
                        <p><strong>Fecha:</strong> ${formatDate(appt.date)}</p>
                        <p><strong>Hora:</strong> ${formatTime(appt.time)}</p>
                        <p><strong>Peluquero:</strong> ${appt.groomerName}</p>
                        <p><strong>Precio:</strong> $${appt.price.toFixed(2)}</p>
                    </div>
                    <div class="appointment-status status-${appt.status}">${appt.status}</div>
                    <div class="appointment-actions">
                        ${appt.status === 'Pendiente' || appt.status === 'Confirmado' ? `<button onclick="cancelAppointment(${appt.id})">Cancelar</button>` : ''}
                    </div>
                `;
                appointmentsListContainer.appendChild(appointmentCard);
            });

        } catch (error) {
            console.error('Error al cargar los turnos:', error);
            appointmentsListContainer.innerHTML = '<p class="error-message">Error al cargar los turnos. Intente de nuevo más tarde.</p>';
        }
    }

    // Load Available Services for dropdown
    async function loadAvailableServices() {
        appointmentServiceSelect.innerHTML = '<option value="">Cargando servicios...</option>';
        try {
            const response = await fetch('/api/clientapi/services'); // Endpoint para servicios
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            availableServices = await response.json();

            if (availableServices.length === 0) {
                appointmentServiceSelect.innerHTML = '<option value="">No hay servicios disponibles.</option>';
                appointmentServiceSelect.disabled = true;
            } else {
                appointmentServiceSelect.innerHTML = '<option value="">Seleccione un servicio...</option>';
                appointmentServiceSelect.disabled = false;
                availableServices.forEach(service => {
                    const option = document.createElement('option');
                    option.value = service.id;
                    option.textContent = service.name;
                    option.dataset.price = service.price; // Store price for easy access
                    appointmentServiceSelect.appendChild(option);
                });
            }
            checkAppointmentFormEligibility(); // Check if form should be enabled
        } catch (error) {
            console.error('Error al cargar servicios:', error);
            appointmentServiceSelect.innerHTML = '<option value="">Error al cargar servicios.</option>';
            appointmentServiceSelect.disabled = true;
            checkAppointmentFormEligibility();
        }
    }

    // Load Available Groomers for dropdown
    async function loadAvailableGroomers() {
        appointmentGroomerSelect.innerHTML = '<option value="">Cargando peluqueros...</option>';
        try {
            const response = await fetch('/api/clientapi/groomers'); // Endpoint para peluqueros
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            availableGroomers = await response.json();

            if (availableGroomers.length === 0) {
                appointmentGroomerSelect.innerHTML = '<option value="">No hay peluqueros disponibles.</option>';
                appointmentGroomerSelect.disabled = true;
            } else {
                appointmentGroomerSelect.innerHTML = '<option value="">Seleccione un peluquero...</option>';
                appointmentGroomerSelect.disabled = false;
                availableGroomers.forEach(groomer => {
                    const option = document.createElement('option');
                    option.value = groomer.id; // Usar ID del peluquero
                    option.textContent = groomer.name;
                    appointmentGroomerSelect.appendChild(option);
                });
            }
            checkAppointmentFormEligibility(); // Check if form should be enabled
        } catch (error) {
            console.error('Error al cargar peluqueros:', error);
            appointmentGroomerSelect.innerHTML = '<option value="">Error al cargar peluqueros.</option>';
            appointmentGroomerSelect.disabled = true;
            checkAppointmentFormEligibility();
        }
    }

    // Load Pets for Appointment dropdown (only if pets exist)
    async function loadPetsForAppointment() {
        appointmentPetSelect.innerHTML = '<option value="">Cargando mascotas...</option>';
        try {
            const userId = await getCurrentUserId();
            if (!userId) {
                appointmentPetSelect.innerHTML = '<option value="">No se pudieron cargar las mascotas.</option>';
                appointmentPetSelect.disabled = true;
                return;
            }

            const response = await fetch(`/api/clientapi/pets?ownerId=${userId}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            currentUserPets = await response.json(); // Update global pets list

            if (currentUserPets.length === 0) {
                appointmentPetSelect.innerHTML = '<option value="">No tienes mascotas registradas.</option>';
                appointmentPetSelect.disabled = true;
                noPetsForAppointmentMessage.style.display = 'block';
                scheduleAppointmentForm.style.display = 'none'; // Hide form
            } else {
                appointmentPetSelect.innerHTML = '<option value="">Seleccione una mascota...</option>';
                appointmentPetSelect.disabled = false;
                noPetsForAppointmentMessage.style.display = 'none';
                currentUserPets.forEach(pet => {
                    const option = document.createElement('option');
                    option.value = pet.id;
                    option.textContent = pet.name;
                    appointmentPetSelect.appendChild(option);
                });
                checkAppointmentFormEligibility(); // Check if form should be enabled
            }
        } catch (error) {
            console.error('Error al cargar mascotas para el turno:', error);
            appointmentPetSelect.innerHTML = '<option value="">Error al cargar mascotas.</option>';
            appointmentPetSelect.disabled = true;
            noPetsForAppointmentMessage.style.display = 'block';
            scheduleAppointmentForm.style.display = 'none';
        }
    }

    // Function to check if the appointment form should be visible/enabled
    function checkAppointmentFormEligibility() {
        const hasPets = currentUserPets && currentUserPets.length > 0;
        const hasServices = availableServices && availableServices.length > 0;
        const hasGroomers = availableGroomers && availableGroomers.length > 0;

        if (!hasPets) {
            noPetsForAppointmentMessage.style.display = 'block';
            scheduleAppointmentForm.style.display = 'none';
            noServicesOrGroomersMessage.style.display = 'none';
        } else if (!hasServices || !hasGroomers) {
            noPetsForAppointmentMessage.style.display = 'none';
            scheduleAppointmentForm.style.display = 'none';
            noServicesOrGroomersMessage.style.display = 'block';
        } else {
            noPetsForAppointmentMessage.style.display = 'none';
            noServicesOrGroomersMessage.style.display = 'none';
            scheduleAppointmentForm.style.display = 'block';
        }
    }


    // Load Profile
    async function loadProfile() {
        try {
            const userId = await getCurrentUserId();
            if (!userId) {
                console.error('No se pudo cargar el perfil. Usuario no identificado.');
                return;
            }

            const response = await fetch(`/api/accountapi/profile?userId=${userId}`); // Endpoint para obtener el perfil del usuario
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const profileData = await response.json();

            profileName.textContent = profileData.name || 'N/A';
            profileLastName.textContent = profileData.lastName || 'N/A';
            profileEmail.textContent = profileData.email || 'N/A';
            profilePhoneNumber.textContent = profileData.phoneNumber || 'N/A';
            profileBirthDate.textContent = formatDate(profileData.birthDate) || 'N/A';

            // Populate edit form
            document.getElementById('editName').value = profileData.name || '';
            document.getElementById('editLastName').value = profileData.lastName || '';
            document.getElementById('editPhoneNumber').value = profileData.phoneNumber || '';

        } catch (error) {
            console.error('Error al cargar el perfil:', error);
            document.getElementById('profile-section').innerHTML = '<p class="error-message">Error al cargar el perfil. Intente de nuevo más tarde.</p>';
        }
    }

    // --- Form Submissions ---

    // Add Pet Form
    addPetForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        hideFormMessage(addPetMessages);

        const formData = new FormData(this);
        formData.append('NoBreed', document.getElementById('noBreedCheckbox').checked);

        const ownerId = await getCurrentUserId();
        if (!ownerId) {
            showFormMessage(addPetMessages, 'Error: Usuario no identificado. Por favor, inicie sesión nuevamente.', false);
            return;
        }
        formData.append('OwnerId', ownerId);

        try {
            const response = await fetch('/api/clientapi/pets', { // Endpoint para agregar mascotas
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                showFormMessage(addPetMessages, 'Mascota registrada con éxito.', true);
                this.reset();
                showSection('overview-section'); // Go back to overview
            } else {
                const errorData = await response.json();
                let errorMessage = 'Error al registrar mascota. Por favor, verifique los datos.';
                if (errorData && errorData.errors) {
                    errorMessage = Object.values(errorData.errors).flat().join('<br>');
                } else if (typeof errorData === 'string') {
                    errorMessage = errorData;
                }
                showFormMessage(addPetMessages, errorMessage, false);
            }
        } catch (error) {
            console.error('Error de red al registrar mascota:', error);
            showFormMessage(addPetMessages, 'Error de conexión. Intente de nuevo más tarde.', false);
        }
    });

    // Schedule Appointment Form
    scheduleAppointmentForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        hideFormMessage(scheduleAppointmentMessages);

        const petId = parseInt(appointmentPetSelect.value);
        const serviceId = parseInt(appointmentServiceSelect.value);
        const groomerId = parseInt(appointmentGroomerSelect.value); // Obtener ID del peluquero

        const selectedService = availableServices.find(s => s.id === serviceId);
        const selectedGroomer = availableGroomers.find(g => g.id === groomerId); // Encontrar el peluquero

        const appointmentData = {
            PetId: petId,
            Date: document.getElementById('appointmentDate').value,
            Time: document.getElementById('appointmentTime').value + ':00',
            GroomerId: groomerId, // Ahora enviamos el ID del peluquero
            ServiceId: serviceId, // Ahora enviamos el ID del servicio
            Price: selectedService ? parseFloat(selectedService.price) : 0, // Usar el precio del servicio seleccionado
            Status: "Pendiente"
        };

        const clientId = await getCurrentUserId();
        if (!clientId) {
            showFormMessage(scheduleAppointmentMessages, 'Error: Usuario no identificado. Por favor, inicie sesión nuevamente.', false);
            return;
        }
        appointmentData.ClientId = clientId;

        // Validaciones básicas del lado del cliente
        let hasError = false;
        document.querySelectorAll('#scheduleAppointmentForm .error-message').forEach(el => el.textContent = '');

        if (!appointmentData.PetId) {
            document.getElementById('errorAppointmentPet').textContent = 'Seleccione una mascota.';
            hasError = true;
        }
        if (!appointmentData.Date) {
            document.getElementById('errorAppointmentDate').textContent = 'Seleccione una fecha.';
            hasError = true;
        } else {
            const selectedDate = new Date(appointmentData.Date);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            if (selectedDate < today) {
                document.getElementById('errorAppointmentDate').textContent = 'La fecha no puede ser en el pasado.';
                hasError = true;
            }
        }
        if (!appointmentData.Time || appointmentData.Time === ':00') {
            document.getElementById('errorAppointmentTime').textContent = 'Seleccione una hora.';
            hasError = true;
        }
        if (!appointmentData.GroomerId) {
            document.getElementById('errorAppointmentGroomer').textContent = 'Seleccione un peluquero.';
            hasError = true;
        }
        if (!appointmentData.ServiceId) {
            document.getElementById('errorAppointmentService').textContent = 'Seleccione un servicio.';
            hasError = true;
        }

        if (hasError) {
            showFormMessage(scheduleAppointmentMessages, 'Por favor, complete todos los campos obligatorios correctamente.', false);
            return;
        }

        try {
            const response = await fetch('/api/clientapi/appointments', { // Endpoint para agendar turnos
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(appointmentData)
            });

            if (response.ok) {
                showFormMessage(scheduleAppointmentMessages, 'Turno agendado con éxito.', true);
                this.reset();
                appointmentPriceInput.value = '';
                loadAppointments(); // Recargar la lista de turnos
                loadPets(); // Actualizar el "próximo turno" en las cards de mascotas
            } else {
                const errorData = await response.json();
                let errorMessage = 'Error al agendar turno. Por favor, verifique los datos.';
                if (errorData && errorData.errors) {
                    errorMessage = Object.values(errorData.errors).flat().join('<br>');
                } else if (typeof errorData === 'string') {
                    errorMessage = errorData;
                }
                showFormMessage(scheduleAppointmentMessages, errorMessage, false);
            }
        } catch (error) {
            console.error('Error de red al agendar turno:', error);
            showFormMessage(scheduleAppointmentMessages, 'Error de conexión. Intente de nuevo más tarde.', false);
        }
    });

    // Toggle Breed input
    document.getElementById('noBreedCheckbox').addEventListener('change', function () {
        const petBreedInput = document.getElementById('petBreed');
        if (this.checked) {
            petBreedInput.value = 'Mestizo';
            petBreedInput.setAttribute('readonly', 'readonly');
        } else {
            petBreedInput.value = '';
            petBreedInput.removeAttribute('readonly');
        }
    });

    // Update estimated price based on service selection
    appointmentServiceSelect.addEventListener('change', function () {
        const selectedServiceId = parseInt(this.value);
        const selectedService = availableServices.find(s => s.id === selectedServiceId);
        appointmentPriceInput.value = selectedService ? selectedService.price.toFixed(2) : '0.00';
    });


    // --- Profile Edit Logic (Mantener, aunque no esté en el flujo principal) ---
    editProfileBtn.addEventListener('click', function () {
        editProfileFormContainer.style.display = 'block';
        hideFormMessage(editProfileMessages);
        hideFormMessage(changePasswordMessages);
    });

    cancelEditProfile.addEventListener('click', function () {
        editProfileFormContainer.style.display = 'none';
        editProfileForm.reset();
        changePasswordForm.reset();
        hideFormMessage(editProfileMessages);
        hideFormMessage(changePasswordMessages);
    });

    editProfileForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        hideFormMessage(editProfileMessages);

        const userId = await getCurrentUserId();
        if (!userId) {
            showFormMessage(editProfileMessages, 'Error: Usuario no identificado.', false);
            return;
        }

        const updatedProfile = {
            Name: document.getElementById('editName').value,
            LastName: document.getElementById('editLastName').value,
            PhoneNumber: document.getElementById('editPhoneNumber').value,
        };

        try {
            const response = await fetch(`/api/accountapi/profile/${userId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updatedProfile)
            });

            if (response.ok) {
                showFormMessage(editProfileMessages, 'Perfil actualizado con éxito.', true);
                loadProfile();
                editProfileFormContainer.style.display = 'none';
            } else {
                const errorData = await response.json();
                let errorMessage = 'Error al actualizar perfil.';
                if (errorData && errorData.errors) {
                    errorMessage = Object.values(errorData.errors).flat().join('<br>');
                } else if (typeof errorData === 'string') {
                    errorMessage = errorData;
                }
                showFormMessage(editProfileMessages, errorMessage, false);
            }
        } catch (error) {
            console.error('Error de red al actualizar perfil:', error);
            showFormMessage(editProfileMessages, 'Error de conexión. Intente de nuevo más tarde.', false);
        }
    });

    changePasswordForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        hideFormMessage(changePasswordMessages);

        const userId = await getCurrentUserId();
        if (!userId) {
            showFormMessage(changePasswordMessages, 'Error: Usuario no identificado.', false);
            return;
        }

        const passwordData = {
            OldPassword: document.getElementById('oldPassword').value,
            NewPassword: document.getElementById('newPassword').value,
            ConfirmNewPassword: document.getElementById('confirmNewPassword').value
        };

        if (passwordData.NewPassword !== passwordData.ConfirmNewPassword) {
            showFormMessage(changePasswordMessages, 'La nueva contraseña y la confirmación no coinciden.', false);
            return;
        }
        if (passwordData.NewPassword.length < 8 || !/[A-Z]/.test(passwordData.NewPassword) || !/[a-z]/.test(passwordData.NewPassword) || !/[0-9]/.test(passwordData.NewPassword)) {
            showFormMessage(changePasswordMessages, 'La nueva contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.', false);
            return;
        }

        try {
            const response = await fetch(`/api/accountapi/change-password/${userId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(passwordData)
            });

            if (response.ok) {
                showFormMessage(changePasswordMessages, 'Contraseña cambiada con éxito.', true);
                this.reset();
            } else {
                const errorData = await response.json();
                let errorMessage = 'Error al cambiar contraseña.';
                if (errorData && errorData.errors) {
                    errorMessage = Object.values(errorData.errors).flat().join('<br>');
                } else if (typeof errorData === 'string') {
                    errorMessage = errorData;
                }
                showFormMessage(changePasswordMessages, errorMessage, false);
            }
        } catch (error) {
            console.error('Error de red al cambiar sesión:', error);
            showFormMessage(changePasswordMessages, 'Error de conexión. Intente de nuevo más tarde.', false);
        }
    });

    // --- Pet/Appointment Actions (Edit/Delete/Cancel) ---
    // These functions need to be global or attached to window because they are called from inline onclick.
    window.editPet = async function (petId) {
        alert(`Funcionalidad de editar mascota (ID: ${petId}) - ¡Pendiente de implementar!`);
        // Aquí podrías cargar los datos de la mascota en el formulario de agregar mascota
        // y cambiar el botón de "Registrar" a "Actualizar".
        // O abrir un modal/nueva sección de edición.
    };

    window.deletePet = async function (petId) {
        if (!confirm('¿Estás seguro de que quieres eliminar esta mascota?')) {
            return;
        }

        try {
            const response = await fetch(`/api/clientapi/pets/${petId}`, {
                method: 'DELETE'
            });

            if (response.ok) {
                alert('Mascota eliminada con éxito.');
                loadPets(); // Recargar la lista de mascotas
                // No es necesario loadPetsForAppointment porque loadPets ya lo actualiza indirectamente
            } else {
                const errorData = await response.json();
                alert(`Error al eliminar mascota: ${errorData.message || 'Error desconocido'}`);
            }
        } catch (error) {
            console.error('Error de red al eliminar mascota:', error);
            alert('Error de conexión al eliminar mascota.');
        }
    };

    window.cancelAppointment = async function (appointmentId) {
        if (!confirm('¿Estás seguro de que quieres cancelar este turno?')) {
            return;
        }

        try {
            const response = await fetch(`/api/clientapi/appointments/${appointmentId}/cancel`, {
                method: 'PUT'
            });

            if (response.ok) {
                alert('Turno cancelado con éxito.');
                loadAppointments(); // Recargar la lista de turnos
                loadPets(); // Para actualizar los próximos turnos en las cards de mascotas
            } else {
                const errorData = await response.json();
                alert(`Error al cancelar turno: ${errorData.message || 'Error desconocido'}`);
            }
        } catch (error) {
            console.error('Error de red al cancelar turno:', error);
            alert('Error de conexión al cancelar turno.');
        }
    };

    // --- Logout ---
    logoutButton.addEventListener('click', async function (e) {
        e.preventDefault();
        try {
            // Asumiendo que tu acción de Logout en AccountController maneja esto
            const response = await fetch('/Account/Logout', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // Incluye el token Anti-Forgery si tu acción de Logout lo requiere
                    // 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            });

            if (response.ok) {
                window.location.href = '/'; // Redirigir a la página de inicio/login
            } else {
                console.error('Error al cerrar sesión:', response.statusText);
                alert('No se pudo cerrar la sesión. Intente de nuevo.');
            }
        } catch (error) {
            console.error('Error de red al cerrar sesión:', error);
            alert('Error de conexión al cerrar sesión.');
        }
    });

    // Initial Load: Show overview and load initial data
    showSection('overview-section');
    loadPets(); // This will determine initial state of the dashboard
});