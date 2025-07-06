document.addEventListener('DOMContentLoaded', function () {
    const clientId = document.getElementById('clientId')?.value;
    const sidebarButtons = document.querySelectorAll('.sidebar-button');
    const dashboardSections = document.querySelectorAll('.dashboard-section');

    const noPetsMessage = document.getElementById('noPetsMessage');
    const petsOverviewContainer = document.getElementById('petsOverviewContainer');

    const addPetForm = document.getElementById('addPetForm');
    const addPetMessages = document.getElementById('addPetMessages');
    const noBreedCheckbox = document.getElementById('noBreedCheckbox');
    const petBreedInput = document.getElementById('petBreed');

    const scheduleAppointmentForm = document.getElementById('scheduleAppointmentForm');
    const scheduleAppointmentMessages = document.getElementById('scheduleAppointmentMessages');
    const appointmentPetSelect = document.getElementById('appointmentPet');
    const appointmentDateInput = document.getElementById('appointmentDate');
    const appointmentTimeInput = document.getElementById('appointmentTime');
    const appointmentGroomerSelect = document.getElementById('appointmentGroomer');
    const appointmentServiceSelect = document.getElementById('appointmentService');
    const appointmentPriceInput = document.getElementById('appointmentPrice');
    const btnPeluqueria = document.getElementById('btnPeluqueria'); // El botón de la sidebar
    const noPetsForAppointmentMessage = document.getElementById('noPetsForAppointmentMessage');
    const noServicesOrGroomersMessage = document.getElementById('noServicesOrGroomersMessage');
    const appointmentsListContainer = document.getElementById('appointmentsListContainer');

    const editProfileBtn = document.getElementById('editProfileBtn');
    const cancelEditProfileBtn = document.getElementById('cancelEditProfile');
    const editProfileFormContainer = document.getElementById('editProfileFormContainer');
    const editProfileForm = document.getElementById('editProfileForm');
    const changePasswordForm = document.getElementById('changePasswordForm');
    const editProfileMessages = document.getElementById('editProfileMessages');
    const changePasswordMessages = document.getElementById('changePasswordMessages');

    // --- Funciones de Utilidad ---

    // Función para mostrar mensajes de éxito o error
    function showMessage(element, message, type) {
        element.textContent = message;
        element.className = `form-messages ${type}`; // Reemplaza clases existentes
        element.style.display = 'block';
        setTimeout(() => {
            element.style.display = 'none';
        }, 5000); // Ocultar después de 5 segundos
    }

    // Función para cambiar de sección del dashboard
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
        window.scrollTo({ top: 0, behavior: 'smooth' }); // Scroll al inicio de la página
    }

    // --- Gestión de Secciones (Botones de Sidebar) ---
    sidebarButtons.forEach(button => {
        button.addEventListener('click', function () {
            const section = this.dataset.section;
            if (section) {
                showSection(`${section}-section`);
                if (section === 'overview') {
                    loadPets(); // Recargar mascotas al volver a la vista general
                } else if (section === 'schedule-appointment') {
                    if (btnPeluqueria.disabled) { // Si el botón de peluquería está deshabilitado
                        showSection('add-pet-section'); // Forzar la sección de agregar mascota
                        showMessage(addPetMessages, 'Para agendar un turno, primero debes registrar una mascota.', 'warning');
                    } else {
                        loadPetsForAppointment();
                        loadGroomers();
                        loadServices();
                        loadAppointments();
                    }
                } else if (section === 'profile') {
                    loadProfileData();
                }
            }
        });
    });

    // Manejar el botón de "Registrar mi primera mascota" en la sección de "overview"
    if (noPetsMessage) {
        noPetsMessage.addEventListener('click', function (event) {
            if (event.target.classList.contains('btn-primary')) {
                showSection('add-pet-section');
            }
        });
    }

    // Manejar el enlace en el mensaje de error de "Agendar Turno"
    if (noPetsForAppointmentMessage) {
        noPetsForAppointmentMessage.addEventListener('click', function (event) {
            if (event.target.dataset.section === 'add-pet') {
                event.preventDefault(); // Evita el comportamiento de enlace por defecto
                showSection('add-pet-section');
            }
        });
    }


    // --- Lógica de Mascotas --
    // Cargar y mostrar mascotas

    async function loadPets() {
        const clientId = document.getElementById('clientId').value;

        if (!clientId) {
            console.error('Client ID no encontrado.');
            return;
        }

        try {
            const response = await fetch(`/api/ClientApi/pets`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include' // Asegura que la cookie de autenticación se incluya
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error(`Error al cargar mascotas: HTTP error! status: ${response.status}`, errorText);
                throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
            }

            const pets = await response.json();
            displayPets(pets);
            checkPetsAvailability(pets.length > 0);

        } catch (error) {
            console.error('Error al cargar mascotas:', error);
            // Mostrar mensaje al usuario si querés
        }
    }

    // Mostrar las tarjetas de mascotas
    function displayPets(pets) {
        petsOverviewContainer.innerHTML = ''; // Limpiar contenedor
        if (pets && pets.length > 0) {
            noPetsMessage.style.display = 'none';
            petsOverviewContainer.style.display = 'grid'; // Mostrar grid
            pets.forEach(pet => {
                const petCard = document.createElement('div');
                petCard.className = 'pet-card';
                petCard.innerHTML = `
                <div class="pet-card-header">${pet.name}</div>
                <div class="pet-card-body">
                    <img src="${pet.profilePicUrl || '/img/default-pet.png'}" alt="Foto de ${pet.name}">
                    <h3>${pet.name}</h3>
                    <p><strong>Raza:</strong> ${pet.breed || 'Mestizo'}</p>
                    <p><strong>Tamaño:</strong> ${pet.size}</p>
                    <p><strong>Sexo:</strong> ${pet.sex}</p>
                    <p><strong>Peso:</strong> ${pet.weight} kg</p>
                    <p><strong>Nacimiento:</strong> ${new Date(pet.birthDate).toLocaleDateString()}</p>
                    <p><strong>Castrado:</strong> ${pet.castrated ? 'Sí' : 'No'}</p>
                    <p><strong>Alergias:</strong> ${pet.allergies ? 'Sí' : 'No'}</p>
                </div>
                <div class="pet-card-actions">
                    <button class="edit-pet-btn" data-pet-id="${pet.id}">Editar</button>
                    <button class="delete-pet-btn" data-pet-id="${pet.id}">Eliminar</button>
                </div>
            `;
                petsOverviewContainer.appendChild(petCard);
            });

            document.querySelector('#noPetsMessage .btn-primary').style.display = 'none';

            // Reasignar listeners luego de renderizar
            petsOverviewContainer.querySelectorAll('.edit-pet-btn').forEach(button => {
                button.addEventListener('click', async function () {
                    const petId = this.dataset.petId;
                    try {
                        const response = await fetch(`/api/ClientApi/pets/${petId}`);
                        if (!response.ok) throw new Error('No se pudo cargar la mascota.');
                        const pet = await response.json();
                        fillEditPetForm(pet);
                        showSection('edit-pet-section');

                        const cancelEditBtn = document.querySelector('[data-section="cancel-edit-pet"]');
                        if (cancelEditBtn) {
                            cancelEditBtn.addEventListener('click', () => {
                                showSection('overview-section');
                            });
                        }
                    } catch (error) {
                        console.error('Error al cargar mascota:', error);
                    }
                });
            });

            petsOverviewContainer.querySelectorAll('.delete-pet-btn').forEach(button => {
                button.addEventListener('click', async function () {
                    const petId = this.dataset.petId;
                    if (confirm('¿Estás seguro de que deseas eliminar esta mascota?')) {
                        try {
                            const response = await fetch(`/api/ClientApi/pets/${petId}`, {
                                method: 'DELETE',
                                credentials: 'include'
                            });

                            if (response.ok) {
                                showMessage(addPetMessages, 'Mascota eliminada con éxito!', 'success');
                                loadPets();
                            } else {
                                const errorData = await response.text();
                                showMessage(addPetMessages, `Error al eliminar la mascota: ${errorData}`, 'error');
                            }
                        } catch (error) {
                            console.error('Error al eliminar mascota:', error);
                        }
                    }
                });
            });

        } else {
            noPetsMessage.style.display = 'block';
            petsOverviewContainer.style.display = 'none';
            document.querySelector('#noPetsMessage .btn-primary').style.display = 'block';
        }
    }

    // Asegurate de tener definida esta función si usas el formulario de edición
    function fillEditPetForm(pet) {
        document.getElementById('editPetId').value = pet.id;
        document.getElementById('editPetName').value = pet.name;
        document.getElementById('editPetBreed').value = pet.breed;
        document.getElementById('editPetSize').value = pet.size;
        document.getElementById('editPetSex').value = pet.sex;
        document.getElementById('editPetWeight').value = pet.weight;
        document.getElementById('editPetBirthDate').value = pet.birthDate.split('T')[0];
        document.getElementById('editPetCastrated').checked = pet.castrated;
        document.getElementById('editPetAllergies').checked = pet.allergies;
    }

    //enviar formulario edicion
    document.getElementById('editPetForm').addEventListener('submit', async function (e) {
        e.preventDefault();

        const petId = document.getElementById('editPetId').value;
        const form = e.target;
        const formData = new FormData(form);

        try {
            const response = await fetch(`/api/ClientApi/pets/${petId}`, {
                method: 'PUT',
                body: formData
            });

            const messagesContainer = document.getElementById('editPetMessages');
            messagesContainer.innerHTML = '';

            if (response.ok) {
                messagesContainer.innerHTML = '<p class="success-message">Mascota actualizada con éxito.</p>';
                loadPets(); // Volver a cargar la lista
                showSection('overview-section');
            } else {
                const errorText = await response.text();
                messagesContainer.innerHTML = `<p class="error-message">Error: ${errorText}</p>`;
            }
        } catch (error) {
            console.error('Error actualizando mascota:', error);
            document.getElementById('editPetMessages').innerHTML = '<p class="error-message">Error al actualizar la mascota.</p>';
        }
    });



    // Deshabilitar/Habilitar el botón de Peluquería y mostrar mensajes
    function checkPetsAvailability(hasPets) {
        if (hasPets) {
            btnPeluqueria.disabled = false;
            btnPeluqueria.classList.remove('disabled-button'); // Puedes añadir una clase para estilos
            noPetsForAppointmentMessage.style.display = 'none';
            scheduleAppointmentForm.style.display = 'block'; // Mostrar el formulario
        } else {
            btnPeluqueria.disabled = true;
            btnPeluqueria.classList.add('disabled-button'); // Puedes añadir una clase para estilos
            noPetsForAppointmentMessage.style.display = 'block';
            scheduleAppointmentForm.style.display = 'none'; // Ocultar el formulario
        }
    }


    // Lógica para "Sin raza (mestizo)" checkbox
    if (noBreedCheckbox && petBreedInput) {
        noBreedCheckbox.addEventListener('change', function () {
            if (this.checked) {
                petBreedInput.value = 'Mestizo';
                petBreedInput.disabled = true;
            } else {
                petBreedInput.value = '';
                petBreedInput.disabled = false;
            }
        });
    }

    // Enviar formulario de añadir mascota
    if (addPetForm) {
        addPetForm.addEventListener('submit', async function (event) {
            event.preventDefault();
            addPetMessages.style.display = 'none'; // Ocultar mensajes previos

            const formData = new FormData(this);

            try {
                const response = await fetch('/api/ClientApi/pets', {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    showMessage(addPetMessages, 'Mascota registrada con éxito!', 'success');
                    addPetForm.reset(); // Limpiar el formulario
                    petBreedInput.disabled = false;
                    noBreedCheckbox.checked = false;
                    loadPets(); // Recargar lista
                    showSection('overview-section');
                } else {
                    const errorData = await response.json();
                    const errorMessage = errorData.errors
                        ? Object.values(errorData.errors).flat().join('<br>')
                        : (errorData.message || 'Error al registrar la mascota.');
                    showMessage(addPetMessages, `Error: ${errorMessage}`, 'error');
                }
            } catch (error) {
                console.error('Error:', error);
                showMessage(addPetMessages, 'Error de conexión o del servidor al registrar la mascota.', 'error');
            }
        });
    }

    // Botón Cancelar en el formulario de agregar mascota
    document.querySelector('button[data-section="cancel-add-pet"]')?.addEventListener('click', function () {
        addPetForm.reset();
        petBreedInput.disabled = false;
        noBreedCheckbox.checked = false;
        showSection('overview-section');
    });

    // --- Lógica de Turnos de Peluquería ---

    async function loadPetsForAppointment() {
        appointmentPetSelect.innerHTML = '<option value="">Seleccione una mascota...</option>';
        try {
            const response = await fetch('/api/ClientApi/pets');
            if (!response.ok) throw new Error('Failed to load pets');
            const pets = await response.json();

            if (pets.length > 0) {
                pets.forEach(pet => {
                    const option = document.createElement('option');
                    option.value = pet.id;
                    option.textContent = pet.name;
                    appointmentPetSelect.appendChild(option);
                });
                noPetsForAppointmentMessage.style.display = 'none';
                scheduleAppointmentForm.style.display = 'block';
            } else {
                noPetsForAppointmentMessage.style.display = 'block';
                scheduleAppointmentForm.style.display = 'none';
            }
        } catch (error) {
            console.error('Error loading pets for appointment:', error);
            showMessage(scheduleAppointmentMessages, 'Error al cargar las mascotas para el turno.', 'error');
        }
    }


    async function loadGroomers() {
        appointmentGroomerSelect.innerHTML = '<option value="">Seleccione un peluquero...</option>';
        try {
            const response = await fetch('/api/ClientApi/groomers');
            if (!response.ok) throw new Error('Failed to load groomers');
            const groomers = await response.json();
            if (groomers.length > 0) {
                groomers.forEach(groomer => {
                    const option = document.createElement('option');
                    option.value = groomer.id;
                    option.textContent = groomer.name;
                    appointmentGroomerSelect.appendChild(option);
                });
                noServicesOrGroomersMessage.style.display = 'none';
            } else {
                noServicesOrGroomersMessage.style.display = 'block';
            }
        } catch (error) {
            console.error('Error loading groomers:', error);
            showMessage(scheduleAppointmentMessages, 'Error al cargar los peluqueros.', 'error');
            noServicesOrGroomersMessage.style.display = 'block';
        }
    }

    async function loadServices() {
        appointmentServiceSelect.innerHTML = '<option value="">Seleccione un servicio...</option>';
        try {
            const response = await fetch('/api/ClientApi/services');
            if (!response.ok) throw new Error('Failed to load services');
            const services = await response.json();
            if (services.length > 0) {
                services.forEach(service => {
                    const option = document.createElement('option');
                    option.value = service.id;
                    option.textContent = service.nombre;
                    option.dataset.price = service.precio;
                    appointmentServiceSelect.appendChild(option);
                });
                noServicesOrGroomersMessage.style.display = 'none';
            } else {
                noServicesOrGroomersMessage.style.display = 'block';
            }
        } catch (error) {
            console.error('Error loading services:', error);
            showMessage(scheduleAppointmentMessages, 'Error al cargar los servicios.', 'error');
            noServicesOrGroomersMessage.style.display = 'block';
        }
    }

    // Actualizar precio estimado cuando se selecciona un servicio
    if (appointmentServiceSelect && appointmentPriceInput) {
        appointmentServiceSelect.addEventListener('change', function () {
            const selectedOption = this.options[this.selectedIndex];
            const price = selectedOption.dataset.price;
            appointmentPriceInput.value = price ? `$${parseFloat(price).toFixed(2)}` : '';
        });
    }

    async function loadAppointments() {
        appointmentsListContainer.innerHTML = '<p>Cargando turnos...</p>';
        try {
            const response = await fetch('/api/ClientApi/appointments');
            if (!response.ok) throw new Error('Failed to load appointments');
            const appointments = await response.json();
            displayAppointments(appointments);
        } catch (error) {
            console.error('Error loading appointments:', error);
            appointmentsListContainer.innerHTML = '<p>Error al cargar los turnos.</p>';
        }
    }


    function displayAppointments(appointments) {
        appointmentsListContainer.innerHTML = '';
        if (appointments.length === 0) {
            appointmentsListContainer.innerHTML = '<p>No tienes turnos agendados.</p>';
            return;
        }

        appointments.forEach(appt => {
            const appointmentCard = document.createElement('div');
            appointmentCard.className = 'appointment-card';
            appointmentCard.innerHTML = `
                <div class="appointment-info">
                    <h3>Turno para ${appt.petName}</h3>
                    <p><strong>Fecha:</strong> ${new Date(appt.date).toLocaleDateString()}</p>
                    <p><strong>Hora:</strong> ${appt.time}</p>
                    <p><strong>Servicio:</strong> ${appt.serviceName}</p>
                    <p><strong>Peluquero:</strong> ${appt.groomerName}</p>
                </div>
                <span class="appointment-status status-${appt.status}">${appt.status}</span>
                <div class="appointment-actions">
                    ${appt.status === 'Pendiente' ? `<button class="cancel-appointment-btn" data-appointment-id="${appt.id}">Cancelar</button>` : ''}
                </div>
            `;
            appointmentsListContainer.appendChild(appointmentCard);
        });

        // Añadir listeners a los botones de cancelar
        appointmentsListContainer.querySelectorAll('.cancel-appointment-btn').forEach(button => {
            button.addEventListener('click', async function () {
                const appointmentId = this.dataset.appointmentId;
                if (confirm('¿Estás seguro de que quieres cancelar este turno?')) {
                    try {
                        const response = await fetch(`/api/ClientApi/appointments/${appointmentId}/cancel`, {
                            method: 'PUT',
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value // Incluir token
                            }
                        });
                        if (response.ok) {
                            showMessage(scheduleAppointmentMessages, 'Turno cancelado con éxito.', 'success');
                            loadAppointments(); // Recargar la lista de turnos
                        } else {
                            const errorData = await response.json();
                            showMessage(scheduleAppointmentMessages, `Error al cancelar turno: ${errorData.message || 'Error desconocido'}`, 'error');
                        }
                    } catch (error) {
                        console.error('Error cancelling appointment:', error);
                        showMessage(scheduleAppointmentMessages, 'Error de conexión al cancelar turno.', 'error');
                    }
                }
            });
        });
    }


    // Enviar formulario de agendar turno
    if (scheduleAppointmentForm) {
        scheduleAppointmentForm.addEventListener('submit', async function (event) {
            event.preventDefault();
            scheduleAppointmentMessages.style.display = 'none'; // Ocultar mensajes previos

            const time = appointmentTimeInput.value;
            const timeFormatted = time.length === 5 ? `${time}:00` : time;

            const formData = {
                PetId: appointmentPetSelect.value,
                Date: appointmentDateInput.value,
                Time: timeFormatted,
                GroomerId: appointmentGroomerSelect.value,
                ServiceId: appointmentServiceSelect.value
            };

            // Validación básica de campos
            let isValid = true;
            if (!formData.PetId) { document.getElementById('errorAppointmentPet').textContent = 'Seleccione una mascota.'; isValid = false; } else { document.getElementById('errorAppointmentPet').textContent = ''; }
            if (!formData.Date) { document.getElementById('errorAppointmentDate').textContent = 'Seleccione una fecha.'; isValid = false; } else { document.getElementById('errorAppointmentDate').textContent = ''; }
            if (!formData.Time) { document.getElementById('errorAppointmentTime').textContent = 'Seleccione una hora.'; isValid = false; } else { document.getElementById('errorAppointmentTime').textContent = ''; }
            if (!formData.GroomerId) { document.getElementById('errorAppointmentGroomer').textContent = 'Seleccione un peluquero.'; isValid = false; } else { document.getElementById('errorAppointmentGroomer').textContent = ''; }
            if (!formData.ServiceId) { document.getElementById('errorAppointmentService').textContent = 'Seleccione un servicio.'; isValid = false; } else { document.getElementById('errorAppointmentService').textContent = ''; }

            if (!isValid) {
                showMessage(scheduleAppointmentMessages, 'Por favor, complete todos los campos requeridos.', 'error');
                return;
            }

            try {
                const response = await fetch('/api/ClientApi/appointments', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value // Incluir token
                    },
                    body: JSON.stringify(formData)
                });

                if (response.ok) {
                    showMessage(scheduleAppointmentMessages, 'Turno agendado con éxito!', 'success');
                    scheduleAppointmentForm.reset(); // Limpiar el formulario
                    loadAppointments(); // Recargar la lista de turnos
                } else {
                    const contentType = response.headers.get("content-type");
                    let errorMessage = 'Error al agendar el turno.';

                    if (contentType && contentType.includes("application/json")) {
                        const errorData = await response.json();
                        errorMessage = errorData.errors
                            ? Object.values(errorData.errors).flat().join('<br>')
                            : (errorData.message || errorMessage);
                    } else {
                        const errorText = await response.text();
                        errorMessage = `Error: ${errorText}`;
                    }

                    showMessage(scheduleAppointmentMessages, errorMessage, 'error');

                }
            } catch (error) {
                console.error('Error:', error);
                showMessage(scheduleAppointmentMessages, 'Error de conexión o del servidor al agendar el turno.', 'error');
            }
        });
    }

    // --- Lógica de Perfil ---

    async function loadProfileData() {
        if (!clientId) {
            console.error('Client ID no encontrado para cargar perfil.');
            return;
        }
        try {
            const response = await fetch(`/api/ClientApi/clientprofile`);
            if (!response.ok) throw new Error('Failed to load profile data');
            const profile = await response.json();

            document.getElementById('profileName').textContent = profile.name;
            document.getElementById('profileLastName').textContent = profile.lastName;
            document.getElementById('profileEmail').textContent = profile.email;
            document.getElementById('profilePhoneNumber').textContent = profile.phoneNumber || 'N/A';
            document.getElementById('profileBirthDate').textContent = profile.birthDate ? new Date(profile.birthDate).toLocaleDateString() : 'N/A';

            // Precargar el formulario de edición
            document.getElementById('editName').value = profile.name;
            document.getElementById('editLastName').value = profile.lastName;
            document.getElementById('editPhoneNumber').value = profile.phoneNumber || '';

        } catch (error) {
            console.error('Error al cargar datos del perfil:', error);
            // Podrías mostrar un mensaje de error en la sección de perfil
        }
    }

    if (editProfileBtn) {
        editProfileBtn.addEventListener('click', function () {
            editProfileFormContainer.style.display = 'block';
            editProfileBtn.style.display = 'none'; // Ocultar el botón de editar
            loadProfileData(); // Asegurarse de que los datos estén cargados en el formulario
        });
    }

    if (cancelEditProfileBtn) {
        cancelEditProfileBtn.addEventListener('click', function () {
            editProfileFormContainer.style.display = 'none';
            editProfileBtn.style.display = 'block'; // Mostrar el botón de editar
            editProfileForm.reset(); // Limpiar el formulario
            editProfileMessages.style.display = 'none';
        });
    }

    if (editProfileForm) {
        editProfileForm.addEventListener('submit', async function (event) {
            event.preventDefault();
            editProfileMessages.style.display = 'none';

            if (!clientId) {
                showMessage(editProfileMessages, 'Error: No se pudo obtener el ID del cliente.', 'error');
                return;
            }

            const formData = {
                Id: clientId, // Es importante enviar el ID del cliente
                Name: document.getElementById('editName').value,
                LastName: document.getElementById('editLastName').value,
                PhoneNumber: document.getElementById('editPhoneNumber').value,
                // No se edita BirthDate ni Email aquí según tu HTML
            };

            const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            try {
                const response = await fetch(`/api/clientprofile/${clientId}`, { // Usar el ID en la URL
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': antiForgeryToken
                    },
                    body: JSON.stringify(formData)
                });

                if (response.ok) {
                    showMessage(editProfileMessages, 'Perfil actualizado con éxito!', 'success');
                    loadProfileData(); // Recargar los datos mostrados
                    editProfileFormContainer.style.display = 'none';
                    editProfileBtn.style.display = 'block';
                } else {
                    const errorData = await response.json();
                    const errorMessage = errorData.errors ? Object.values(errorData.errors).flat().join('<br>') : (errorData.message || 'Error al actualizar el perfil.');
                    showMessage(editProfileMessages, `Error: ${errorMessage}`, 'error');
                }
            } catch (error) {
                console.error('Error:', error);
                showMessage(editProfileMessages, 'Error de conexión o del servidor al actualizar el perfil.', 'error');
            }
        });
    }

    if (changePasswordForm) {
        changePasswordForm.addEventListener('submit', async function (event) {
            event.preventDefault();
            changePasswordMessages.style.display = 'none';

            const oldPassword = document.getElementById('oldPassword').value;
            const newPassword = document.getElementById('newPassword').value;
            const confirmNewPassword = document.getElementById('confirmNewPassword').value;

            if (newPassword !== confirmNewPassword) {
                showMessage(changePasswordMessages, 'La nueva contraseña y su confirmación no coinciden.', 'error');
                return;
            }

            // Aquí puedes añadir más validaciones de contraseña (longitud, caracteres especiales, etc.)

            const formData = {
                OldPassword: oldPassword,
                NewPassword: newPassword,
                ConfirmNewPassword: confirmNewPassword
            };

            const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            try {
                const response = await fetch(`/api/ClientApi/clientprofile/change-password`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': antiForgeryToken
                    },
                    body: JSON.stringify(formData)
                });

                if (response.ok) {
                    showMessage(changePasswordMessages, 'Contraseña cambiada con éxito!', 'success');
                    changePasswordForm.reset();
                } else {
                    const errorData = await response.json();
                    const errorMessage = errorData.errors ? Object.values(errorData.errors).flat().join('<br>') : (errorData.message || 'Error al cambiar la contraseña.');
                    showMessage(changePasswordMessages, `Error: ${errorMessage}`, 'error');
                }
            } catch (error) {
                console.error('Error:', error);
                showMessage(changePasswordMessages, 'Error de conexión o del servidor al cambiar la contraseña.', 'error');
            }
        });
    }


    // --- Inicialización ---
    // Cargar las mascotas al inicio para determinar el estado del botón de peluquería
    showSection('overview-section'); // Asegurarse de que la sección de mascotas esté activa al cargar
    loadPets();
});