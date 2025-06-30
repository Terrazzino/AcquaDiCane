document.addEventListener('DOMContentLoaded', function () {

    // --- Lógica para Gestionar Servicios (admin-services.html) ---
    const servicesTableBody = document.querySelector('#servicesTable tbody');
    const selectedServiceNameSpan = document.getElementById('selectedServiceName');
    const deleteServiceButton = document.getElementById('deleteServiceButton');
    const modifyServiceButton = document.getElementById('modifyServiceButton');
    const addServiceButton = document.getElementById('addServiceButton'); // Nuevo: Referencia al botón de agregar

    const serviceModal = document.getElementById('serviceModal');
    const closeServiceModalButton = document.getElementById('closeServiceModalButton');
    const serviceModalTitle = document.getElementById('serviceModalTitle');
    const serviceForm = document.getElementById('serviceForm');
    const modalServiceNameInput = document.getElementById('modalServiceNameInput');
    const modalServicePriceInput = document.getElementById('modalServicePriceInput');
    const saveServiceButton = document.getElementById('saveServiceButton');

    // Inicializa services como un array vacío. Los datos se agregarán dinámicamente.
    let services = [];
    let nextServiceId = 1; // El ID comienza en 1 ya que no hay datos preexistentes
    let selectedServiceId = null;
    let editingServiceId = null;

    function renderServices() {
        if (!servicesTableBody) return;

        servicesTableBody.innerHTML = ''; // Limpia la tabla antes de renderizar
        if (services.length === 0) {
            // Opcional: Mostrar un mensaje si no hay servicios
            const row = servicesTableBody.insertRow();
            const cell = row.insertCell(0);
            cell.colSpan = 2; // Cubre todas las columnas de la tabla
            cell.textContent = 'No hay servicios registrados. Utilice el formulario superior para agregar uno.';
            cell.style.textAlign = 'center';
            cell.style.fontStyle = 'italic';
            // Deshabilitar botones de modificar/eliminar si no hay servicios
            if (deleteServiceButton) deleteServiceButton.disabled = true;
            if (modifyServiceButton) modifyServiceButton.disabled = true;
        } else {
            services.forEach(service => {
                const row = servicesTableBody.insertRow();
                row.dataset.id = service.id;
                row.classList.add('service-row');

                row.insertCell(0).textContent = service.name;
                row.insertCell(1).textContent = `$${service.price.toLocaleString('es-AR')}`;
            });
            addServiceRowEventListeners();
            updateSelectedServiceDisplay();
        }
    }

    function addServiceRowEventListeners() {
        document.querySelectorAll('.service-row').forEach(row => {
            row.removeEventListener('click', selectServiceRow);
            row.addEventListener('click', selectServiceRow);
        });
    }

    function selectServiceRow(event) {
        document.querySelectorAll('.service-row').forEach(row => {
            row.classList.remove('selected');
        });

        event.currentTarget.classList.add('selected');
        selectedServiceId = parseInt(event.currentTarget.dataset.id);
        updateSelectedServiceDisplay();
    }

    function updateSelectedServiceDisplay() {
        if (!selectedServiceNameSpan || !deleteServiceButton || !modifyServiceButton) return;

        if (selectedServiceId) {
            const service = services.find(s => s.id === selectedServiceId);
            if (service) {
                selectedServiceNameSpan.textContent = service.name;
                deleteServiceButton.disabled = false;
                modifyServiceButton.disabled = false;
            } else {
                selectedServiceNameSpan.textContent = 'No encontrado';
                selectedServiceId = null;
                deleteServiceButton.disabled = true;
                modifyServiceButton.disabled = true;
            }
        } else {
            selectedServiceNameSpan.textContent = 'Ninguno';
            deleteServiceButton.disabled = true;
            modifyServiceButton.disabled = true;
        }
    }

    function addServiceFromForm() {
        const nameInput = document.getElementById('serviceName');
        const priceInput = document.getElementById('servicePrice');

        if (!nameInput || !priceInput) return;

        const name = nameInput.value.trim();
        const price = parseInt(priceInput.value);

        if (name && !isNaN(price) && price > 0) {
            services.push({ id: nextServiceId++, name: name, price: price });
            renderServices();
            nameInput.value = '';
            priceInput.value = '';
        } else {
            alert('Por favor, ingrese un nombre de servicio y un precio válido para agregar.');
        }
    }

    function openEditServiceModal() {
        if (!selectedServiceId) {
            alert('Por favor, seleccione un servicio de la tabla para modificar.');
            return;
        }
        if (!serviceModalTitle || !modalServiceNameInput || !modalServicePriceInput || !serviceModal) return;

        serviceModalTitle.textContent = 'Modificar Servicio';
        editingServiceId = selectedServiceId;
        const service = services.find(s => s.id === editingServiceId);

        if (service) {
            modalServiceNameInput.value = service.name;
            modalServicePriceInput.value = service.price;
            serviceModal.classList.add('active');
        } else {
            alert('Error: Servicio no encontrado.');
            selectedServiceId = null;
            updateSelectedServiceDisplay();
        }
    }

    function deleteSelectedService() {
        if (!selectedServiceId) {
            alert('Por favor, seleccione un servicio de la tabla para eliminar.');
            return;
        }

        const serviceToDelete = services.find(s => s.id === selectedServiceId);
        if (serviceToDelete && confirm(`¿Está seguro de que desea eliminar el servicio "${serviceToDelete.name}"?`)) {
            services = services.filter(s => s.id !== selectedServiceId);
            renderServices();
            selectedServiceId = null;
            updateSelectedServiceDisplay();
        }
    }

    function saveServiceFromModal(event) {
        event.preventDefault();

        if (!modalServiceNameInput || !modalServicePriceInput || !serviceModal) return;

        const name = modalServiceNameInput.value.trim();
        const price = parseInt(modalServicePriceInput.value);

        if (name && !isNaN(price) && price > 0) {
            if (editingServiceId) {
                const serviceIndex = services.findIndex(s => s.id === editingServiceId);
                if (serviceIndex !== -1) {
                    services[serviceIndex] = { id: editingServiceId, name, price };
                }
            }
            renderServices();
            hideServiceModal();
            selectedServiceId = null;
            updateSelectedServiceDisplay();
        } else {
            alert('Por favor, complete todos los campos con valores válidos.');
        }
    }

    function hideServiceModal() {
        if (serviceModal) serviceModal.classList.remove('active');
    }

    // Inicialización y Listeners para Servicios (solo si los elementos existen en la página)
    if (addServiceButton) { // Usamos un elemento presente solo en la página de servicios
        addServiceButton.addEventListener('click', addServiceFromForm);
        if (deleteServiceButton) deleteServiceButton.addEventListener('click', deleteSelectedService);
        if (modifyServiceButton) modifyServiceButton.addEventListener('click', openEditServiceModal);

        if (closeServiceModalButton) closeServiceModalButton.addEventListener('click', hideServiceModal);
        if (serviceModal) {
            serviceModal.addEventListener('click', function (event) {
                if (event.target === serviceModal) {
                    hideServiceModal();
                }
            });
        }
        if (serviceForm) serviceForm.addEventListener('submit', saveServiceFromModal);

        renderServices(); // Renderiza los servicios al cargar la página (inicialmente vacío)
    }


    // --- Lógica para Gestionar Peluqueros (admin-groomers.html) ---
    const registerGroomerButton = document.getElementById('registerGroomerButton');
    const groomersTableBody = document.querySelector('#groomersTable tbody');
    const groomerModal = document.getElementById('groomerModal');
    const closeGroomerModalButton = document.getElementById('closeGroomerModalButton');
    const groomerModalTitle = document.getElementById('groomerModalTitle');
    const groomerForm = document.getElementById('groomerForm');
    const groomerNameInput = document.getElementById('groomerNameInput');
    const groomerApellidoInput = document.getElementById('groomerApellidoInput');
    const groomerPhoneInput = document.getElementById('groomerPhoneInput');
    const groomerEmailInput = document.getElementById('groomerEmailInput');
    const groomerBirthDateInput = document.getElementById('groomerBirthDateInput');
    const groomerDNIInput = document.getElementById('groomerDNIInput');
    const configureCalendarButton = document.getElementById('configureCalendarButton');
    const saveGroomerButton = document.getElementById('saveGroomerButton');

    const selectedGroomerNameSpan = document.getElementById('selectedGroomerName');
    const deleteGroomerButton = document.getElementById('deleteGroomerButton');
    const modifyGroomerButton = document.getElementById('modifyGroomerButton');

    const calendarModal = document.getElementById('calendarModal');
    const closeCalendarModalButton = document.getElementById('closeCalendarModalButton');
    const calendarForm = document.getElementById('calendarForm');

    // Inicializa groomers como un array vacío. Los datos se agregarán dinámicamente.
    let groomers = [];
    let nextGroomerId = 1; // El ID comienza en 1
    let selectedGroomerId = null;
    let editingGroomerId = null;
    let currentGroomerCalendar = {};

    function renderGroomers() {
        if (!groomersTableBody) return;

        groomersTableBody.innerHTML = ''; // Limpia la tabla antes de renderizar
        if (groomers.length === 0) {
            // Opcional: Mostrar un mensaje si no hay peluqueros
            const row = groomersTableBody.insertRow();
            const cell = row.insertCell(0);
            cell.colSpan = 6; // Cubre todas las columnas de la tabla
            cell.textContent = 'No hay peluqueros registrados. Utilice el formulario superior para agregar uno.';
            cell.style.textAlign = 'center';
            cell.style.fontStyle = 'italic';
            // Deshabilitar botones de modificar/eliminar si no hay peluqueros
            if (deleteGroomerButton) deleteGroomerButton.disabled = true;
            if (modifyGroomerButton) modifyGroomerButton.disabled = true;
        } else {
            groomers.forEach(groomer => {
                const row = groomersTableBody.insertRow();
                row.dataset.id = groomer.id;
                row.classList.add('groomer-row');

                row.insertCell(0).textContent = `${groomer.name} ${groomer.apellido}`;
                row.insertCell(1).textContent = groomer.phone;
                row.insertCell(2).textContent = groomer.email;
                row.insertCell(3).textContent = "N/A"; // Fecha de Inicio (como en tu ejemplo, si no hay campo)
                row.insertCell(4).textContent = formatDateForDisplay(groomer.birthDate);
                row.insertCell(5).textContent = groomer.dni;
            });
            addGroomerRowEventListeners();
            updateSelectedGroomerDisplay();
        }
    }

    function addGroomerRowEventListeners() {
        document.querySelectorAll('.groomer-row').forEach(row => {
            row.removeEventListener('click', selectGroomerRow);
            row.addEventListener('click', selectGroomerRow);
        });
    }

    function selectGroomerRow(event) {
        document.querySelectorAll('.groomer-row').forEach(row => {
            row.classList.remove('selected');
        });

        event.currentTarget.classList.add('selected');
        selectedGroomerId = parseInt(event.currentTarget.dataset.id);
        updateSelectedGroomerDisplay();
    }

    function updateSelectedGroomerDisplay() {
        if (!selectedGroomerNameSpan || !deleteGroomerButton || !modifyGroomerButton) return;

        if (selectedGroomerId) {
            const groomer = groomers.find(g => g.id === selectedGroomerId);
            if (groomer) {
                selectedGroomerNameSpan.textContent = `${groomer.name} ${groomer.apellido}`;
                deleteGroomerButton.disabled = false;
                modifyGroomerButton.disabled = false;
            } else {
                selectedGroomerNameSpan.textContent = 'No encontrado';
                selectedGroomerId = null;
                deleteGroomerButton.disabled = true;
                modifyGroomerButton.disabled = true;
            }
        } else {
            selectedGroomerNameSpan.textContent = 'Ninguno';
            deleteGroomerButton.disabled = true;
            modifyGroomerButton.disabled = true;
        }
    }

    function openRegisterGroomerModal() {
        if (!groomerModalTitle || !groomerForm || !groomerModal) return;

        groomerModalTitle.textContent = 'Registrar Peluquero';
        groomerForm.reset();
        editingGroomerId = null;
        currentGroomerCalendar = {};
        groomerModal.classList.add('active');
    }

    function openEditGroomerModal() {
        if (!selectedGroomerId) {
            alert('Por favor, seleccione un peluquero de la tabla para modificar.');
            return;
        }
        if (!groomerModalTitle || !groomerNameInput || !groomerApellidoInput || !groomerPhoneInput ||
            !groomerEmailInput || !groomerBirthDateInput || !groomerDNIInput || !groomerModal) return;

        groomerModalTitle.textContent = 'Modificar Peluquero';
        editingGroomerId = selectedGroomerId;
        const groomer = groomers.find(g => g.id === editingGroomerId);

        if (groomer) {
            groomerNameInput.value = groomer.name;
            groomerApellidoInput.value = groomer.apellido;
            groomerPhoneInput.value = groomer.phone;
            groomerEmailInput.value = groomer.email;
            groomerBirthDateInput.value = groomer.birthDate;
            groomerDNIInput.value = groomer.dni;
            currentGroomerCalendar = JSON.parse(JSON.stringify(groomer.calendar || {}));
            groomerModal.classList.add('active');
        } else {
            alert('Error: Peluquero no encontrado.');
            selectedGroomerId = null;
            updateSelectedGroomerDisplay();
        }
    }

    function saveGroomer(event) {
        event.preventDefault();

        if (!groomerNameInput || !groomerApellidoInput || !groomerPhoneInput || !groomerEmailInput ||
            !groomerBirthDateInput || !groomerDNIInput || !groomerModal) return;

        const name = groomerNameInput.value.trim();
        const apellido = groomerApellidoInput.value.trim();
        const phone = groomerPhoneInput.value.trim();
        const email = groomerEmailInput.value.trim();
        const birthDate = groomerBirthDateInput.value;
        const dni = groomerDNIInput.value.trim();

        if (!name || !apellido || !phone || !email || !birthDate || !dni) {
            alert('Por favor, complete todos los campos del formulario de registro.');
            return;
        }

        const hasValidSchedule = Object.values(currentGroomerCalendar).some(day => day.start && day.end);
        if (!hasValidSchedule) {
            alert('Debe configurar al menos un día de trabajo con horarios válidos para el peluquero.');
            return;
        }

        if (editingGroomerId) {
            const groomerIndex = groomers.findIndex(g => g.id === editingGroomerId);
            if (groomerIndex !== -1) {
                groomers[groomerIndex] = {
                    id: editingGroomerId,
                    name,
                    apellido,
                    phone,
                    email,
                    birthDate,
                    dni,
                    calendar: currentGroomerCalendar
                };
            }
        } else {
            groomers.push({
                id: nextGroomerId++,
                name,
                apellido,
                phone,
                email,
                birthDate,
                dni,
                calendar: currentGroomerCalendar
            });
        }
        renderGroomers();
        hideGroomerModal();
        selectedGroomerId = null;
        updateSelectedGroomerDisplay();
    }

    function deleteSelectedGroomer() {
        if (!selectedGroomerId) {
            alert('Por favor, seleccione un peluquero de la tabla para eliminar.');
            return;
        }

        const groomerToDelete = groomers.find(g => g.id === selectedGroomerId);
        if (groomerToDelete && confirm(`¿Está seguro de que desea eliminar a ${groomerToDelete.name} ${groomerToDelete.apellido}?`)) {
            groomers = groomers.filter(g => g.id !== selectedGroomerId);
            renderGroomers();
            selectedGroomerId = null;
            updateSelectedGroomerDisplay();
        }
    }

    function hideGroomerModal() {
        if (groomerModal) groomerModal.classList.remove('active');
    }

    function formatDateForDisplay(dateString) {
        if (!dateString) return '';
        const parts = dateString.split('-');
        if (parts.length === 3 && parts[0].length === 4 && parts[1].length === 2 && parts[2].length === 2) {
            return `${parts[2]}/${parts[1]}/${parts[0]}`;
        }
        return dateString;
    }

    const daysOfWeek = ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];

    function openCalendarModal() {
        if (!calendarModal) return;
        calendarModal.classList.add('active');
        populateCalendarForm();
    }

    function closeCalendarModal() {
        if (calendarModal) calendarModal.classList.remove('active');
    }

    function populateCalendarForm() {
        daysOfWeek.forEach(day => {
            const startTimeInput = document.querySelector(`.start-time-input[data-day="${day}"]`);
            const endTimeInput = document.querySelector(`.end-time-input[data-day="${day}"]`);

            if (startTimeInput && endTimeInput) {
                startTimeInput.value = currentGroomerCalendar[day]?.start || '';
                endTimeInput.value = currentGroomerCalendar[day]?.end || '';
            }
        });
    }

    function saveCalendar(event) {
        event.preventDefault();

        let hasValidEntry = false;
        const tempCalendar = {};

        for (const day of daysOfWeek) {
            const startTimeInput = document.querySelector(`.start-time-input[data-day="${day}"]`);
            const endTimeInput = document.querySelector(`.end-time-input[data-day="${day}"]`);

            if (!startTimeInput || !endTimeInput) {
                alert('Error: Elementos de horario no encontrados para un día.');
                return;
            }

            const start = startTimeInput.value;
            const end = endTimeInput.value;

            if (start && end) {
                if (start >= end) {
                    alert(`El horario de finalización para ${day} (${end}) debe ser posterior al horario de inicio (${start}).`);
                    return;
                }
                tempCalendar[day] = { start, end };
                hasValidEntry = true;
            } else if (!start && !end) {
                tempCalendar[day] = { start: '', end: '' };
            } else {
                alert(`Para ${day}, debe completar ambos campos de horario (inicio y fin) o dejar ambos vacíos para un día libre.`);
                return;
            }
        }

        if (!hasValidEntry) {
            alert('Debe configurar al menos un día de trabajo con horarios válidos.');
            return;
        }

        currentGroomerCalendar = tempCalendar;
        closeCalendarModal();
    }


    // --- Lógica del Botón de Cerrar Sesión (global, en cualquier página de admin) ---
    // El botón de logout ahora está dentro de un formulario en el HTML
    // y se envía directamente al hacer click. El JS ya no necesita crear el formulario.
    const logoutForm = document.getElementById('logoutForm');
    if (logoutForm) {
        // No se necesita un listener específico para el botón si ya es type="submit" dentro del formulario
        // que tiene el action de logout y el antiforja. El navegador manejará el envío.
    }


    // Inicialización y Listeners para Peluqueros (solo si los elementos existen en la página)
    if (registerGroomerButton) {
        registerGroomerButton.addEventListener('click', openRegisterGroomerModal);
        if (closeGroomerModalButton) closeGroomerModalButton.addEventListener('click', hideGroomerModal);
        if (groomerModal) {
            groomerModal.addEventListener('click', function (event) {
                if (event.target === groomerModal) {
                    hideGroomerModal();
                }
            });
        }
        if (groomerForm) groomerForm.addEventListener('submit', saveGroomer);

        if (deleteGroomerButton) deleteGroomerButton.addEventListener('click', deleteSelectedGroomer);
        if (modifyGroomerButton) modifyGroomerButton.addEventListener('click', openEditGroomerModal);

        if (configureCalendarButton) configureCalendarButton.addEventListener('click', openCalendarModal);
        if (closeCalendarModalButton) closeCalendarModalButton.addEventListener('click', closeCalendarModal);
        if (calendarModal) {
            calendarModal.addEventListener('click', function (event) {
                if (event.target === calendarModal) {
                    closeCalendarModal();
                }
            });
        }
        if (calendarForm) calendarForm.addEventListener('submit', saveCalendar);

        renderGroomers(); // Renderiza los peluqueros al cargar la página (inicialmente vacío)
    }
});