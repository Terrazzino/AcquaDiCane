// admin-scripts.js

document.addEventListener('DOMContentLoaded', function () {

    // --- Configuración API ---
    const API_BASE_URL = '/api/PeluqueroApi';

    // --- Referencias a elementos del DOM ---
    const openAddGroomerModalButton = document.getElementById('openAddGroomerModalButton');
    const groomersTableBody = document.querySelector('#groomersTable tbody');

    // Elementos del Modal de Peluquero (ADD/EDIT)
    const groomerModal = document.getElementById('groomerModal');
    const closeGroomerModalButton = document.getElementById('closeGroomerModalButton');
    const groomerModalTitle = document.getElementById('groomerModalTitle');
    const groomerForm = document.getElementById('groomerForm');
    const groomerIdInput = document.getElementById('groomerId');

    const modalGroomerNombreInput = document.getElementById('modalGroomerNombre');
    const modalGroomerApellidoInput = document.getElementById('modalGroomerApellido');
    const modalGroomerEmailInput = document.getElementById('modalGroomerEmail');
    const modalGroomerPhoneInput = document.getElementById('modalGroomerPhone');
    const modalGroomerBirthDateInput = document.getElementById('modalGroomerBirthDate');
    const modalGroomerDNIInput = document.getElementById('modalGroomerDNI');

    const modalGroomerPasswordGroup = document.getElementById('modalGroomerPasswordGroup');
    const modalGroomerPasswordInput = document.getElementById('modalGroomerPassword');
    const modalGroomerActivoGroup = document.getElementById('modalGroomerActivoGroup');
    const modalGroomerEstaActivoCheckbox = document.getElementById('modalGroomerEstaActivo');

    const saveGroomerButton = document.getElementById('saveGroomerButton');
    const cancelGroomerModalButton = document.getElementById('cancelGroomerModalButton');

    // Elementos del Modal de Calendario
    const calendarModal = document.getElementById('calendarModal');
    const closeCalendarModalButton = document.getElementById('closeCalendarModalButton');
    const calendarForm = document.getElementById('calendarForm');
    const configureCalendarButton = document.getElementById('configureCalendarButton');

    // Variable para almacenar el horario del peluquero mientras se edita o crea
    // Formato: { "Lunes": { start: "09:00", end: "17:00" }, "Martes": { start: "10:00", end: "18:00" }, ... }
    let currentGroomerCalendar = {};

    const daysOfWeek = ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo']; // Añadido Domingo


    // --- Funciones de Utilidad (Centralizadas para API y Modales) ---

    // Función de ayuda para obtener el token JWT (si lo usas para autenticación)
    function getAuthToken() {
        // Implementa la lógica para obtener tu token JWT (ej. de localStorage)
        // return localStorage.getItem('authToken');
        return null; // Por ahora, devuelve null si no usas JWT o aún no lo implementas
    }

    // Función centralizada para realizar solicitudes a la API
    async function makeApiRequest(url, method, data = null) {
        const token = getAuthToken();
        const headers = {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        const config = {
            method: method,
            headers: headers,
        };

        if (data) {
            config.body = JSON.stringify(data);
        }

        try {
            const response = await fetch(url, config);

            // Si es 204 No Content (DELETE/PUT exitoso sin retorno)
            if (response.status === 204) {
                return null;
            }

            const responseData = await response.json();

            if (!response.ok) {
                let errorMessage = `Error: ${response.status} ${response.statusText}`;
                if (responseData && typeof responseData === 'object') {
                    if (responseData.errors) { // Errores de ModelState o Identity (C#)
                        // Para ModelState errors (objeto donde cada clave es un campo con un array de errores)
                        const modelErrors = Object.values(responseData.errors).flat();
                        if (modelErrors.length > 0) {
                            errorMessage = modelErrors.join(', ');
                        } else if (Array.isArray(responseData)) { // Para errores de Identity (array de strings)
                            errorMessage = responseData.join('\n');
                        }
                    } else if (responseData.message) { // Mensajes de error personalizados
                        errorMessage = responseData.message;
                    }
                } else if (typeof responseData === 'string') { // Si la respuesta es solo texto de error
                    errorMessage = responseData;
                }
                throw new Error(errorMessage);
            }

            return responseData;
        } catch (error) {
            console.error('Error en la solicitud a la API:', error);
            throw error; // Re-lanza el error para que sea manejado por la función que llama
        }
    }


    function showModal(modalElement) {
        modalElement.classList.add('active');
    }

    function hideModal(modalElement) {
        modalElement.classList.remove('active');
    }

    function formatDateForDisplay(dateString) {
        if (!dateString) return '';
        // Asumiendo que la API devuelve fechas en formato YYYY-MM-DD
        const parts = dateString.split('-');
        if (parts.length === 3) {
            return `${parts[2]}/${parts[1]}/${parts[0]}`; // DD/MM/YYYY
        }
        return dateString;
    }

    // --- Cargar Peluqueros (READ) ---
    async function loadGroomers() {
        if (!groomersTableBody) return;

        try {
            const peluqueros = await makeApiRequest(API_BASE_URL, 'GET');

            groomersTableBody.innerHTML = '';

            if (peluqueros.length === 0) {
                const row = groomersTableBody.insertRow();
                const cell = row.insertCell(0);
                cell.colSpan = 7;
                cell.textContent = 'No hay peluqueros registrados.';
                cell.style.textAlign = 'center';
                cell.style.fontStyle = 'italic';
                return;
            }

            peluqueros.forEach(peluquero => {
                const row = groomersTableBody.insertRow();
                row.insertCell(0).textContent = `${peluquero.nombre} ${peluquero.apellido}`;
                row.insertCell(1).textContent = peluquero.phoneNumber || 'N/A';
                row.insertCell(2).textContent = peluquero.email;
                row.insertCell(3).textContent = formatDateForDisplay(peluquero.fechaNacimiento);
                row.insertCell(4).textContent = peluquero.dni;
                row.insertCell(5).textContent = peluquero.estaActivo ? 'Sí' : 'No';

                const actionsCell = row.insertCell(6);
                const editButton = document.createElement('button');
                editButton.classList.add('btn-info', 'edit-groomer-btn');
                editButton.textContent = 'Modificar';
                editButton.dataset.id = peluquero.id;
                actionsCell.appendChild(editButton);

                const deleteButton = document.createElement('button');
                deleteButton.classList.add('btn-warning', 'delete-groomer-btn');
                deleteButton.textContent = 'Eliminar';
                deleteButton.dataset.id = peluquero.id;
                actionsCell.appendChild(deleteButton);
            });

            document.querySelectorAll('.edit-groomer-btn').forEach(button => {
                button.addEventListener('click', (e) => editGroomer(e.target.dataset.id));
            });
            document.querySelectorAll('.delete-groomer-btn').forEach(button => {
                button.addEventListener('click', (e) => deleteGroomer(e.target.dataset.id));
            });

        } catch (error) {
            console.error('Error al cargar peluqueros:', error);
            groomersTableBody.innerHTML = `<tr><td colspan="7" class="text-center text-danger">Error al cargar peluqueros: ${error.message}</td></tr>`;
        }
    }

    // --- Abrir Modal para Agregar Peluquero ---
    if (openAddGroomerModalButton) {
        openAddGroomerModalButton.addEventListener('click', () => {
            groomerForm.reset();
            groomerIdInput.value = '';
            groomerModalTitle.textContent = 'Registrar Nuevo Peluquero';

            modalGroomerPasswordGroup.style.display = 'block';
            modalGroomerPasswordInput.setAttribute('required', 'required');
            modalGroomerPasswordInput.value = '';

            modalGroomerActivoGroup.style.display = 'none';
            modalGroomerEstaActivoCheckbox.checked = true;

            currentGroomerCalendar = {}; // Limpiar calendario para un nuevo peluquero
            populateCalendarForm(); // Asegurarse de que el modal de calendario se reinicie también

            showModal(groomerModal);
        });
    }

    // --- Abrir Modal para Editar Peluquero ---
    async function editGroomer(id) {
        try {
            const peluquero = await makeApiRequest(`${API_BASE_URL}/${id}`, 'GET');

            groomerIdInput.value = peluquero.id;
            modalGroomerNombreInput.value = peluquero.nombre;
            modalGroomerApellidoInput.value = peluquero.apellido;
            modalGroomerEmailInput.value = peluquero.email;
            modalGroomerPhoneInput.value = peluquero.phoneNumber;
            modalGroomerBirthDateInput.value = peluquero.fechaNacimiento; // Ya debería venir en YYYY-MM-DD
            modalGroomerDNIInput.value = peluquero.dni;
            modalGroomerEstaActivoCheckbox.checked = peluquero.estaActivo;

            groomerModalTitle.textContent = `Editar Peluquero: ${peluquero.nombre} ${peluquero.apellido}`;

            modalGroomerPasswordGroup.style.display = 'none';
            modalGroomerPasswordInput.removeAttribute('required');
            modalGroomerPasswordInput.value = '';

            modalGroomerActivoGroup.style.display = 'block';

            // Cargar el calendario existente del peluquero desde JornadasSemanales
            currentGroomerCalendar = {};
            if (peluquero.jornadasSemanales && peluquero.jornadasSemanales.length > 0) {
                peluquero.jornadasSemanales.forEach(jornada => {
                    currentGroomerCalendar[jornada.dia] = {
                        start: jornada.horaInicio,
                        end: jornada.horaFinal
                    };
                });
            }
            populateCalendarForm(); // Rellenar el formulario del calendario

            showModal(groomerModal);

        } catch (error) {
            console.error('Error al cargar datos del peluquero para edición:', error);
            alert(`No se pudo cargar el peluquero para editar: ${error.message}`);
        }
    }

    // --- Enviar Formulario (CREATE/UPDATE) ---
    if (groomerForm) {
        groomerForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const id = groomerIdInput.value;
            const method = id ? 'PUT' : 'POST';
            const url = id ? `${API_BASE_URL}/${id}` : API_BASE_URL;

            // Transformar currentGroomerCalendar en el formato JornadasSemanales esperado por la API
            const jornadasSemanalesToSend = [];
            for (const day of daysOfWeek) {
                if (currentGroomerCalendar[day] && currentGroomerCalendar[day].start && currentGroomerCalendar[day].end) {
                    jornadasSemanalesToSend.push({
                        Dia: day,
                        HoraInicio: currentGroomerCalendar[day].start,
                        HoraFinal: currentGroomerCalendar[day].end
                    });
                }
            }

            // Validación de que al menos un día tenga horario configurado
            if (jornadasSemanalesToSend.length === 0) {
                alert('Debe configurar al menos un día de trabajo con horarios válidos para el peluquero.');
                return;
            }

            const formData = {
                Id: id ? parseInt(id) : 0,
                Nombre: modalGroomerNombreInput.value,
                Apellido: modalGroomerApellidoInput.value,
                Email: modalGroomerEmailInput.value,
                PhoneNumber: modalGroomerPhoneInput.value,
                DNI: modalGroomerDNIInput.value,
                FechaNacimiento: modalGroomerBirthDateInput.value,
                EstaActivo: id ? modalGroomerEstaActivoCheckbox.checked : true,
                JornadasSemanales: jornadasSemanalesToSend // ¡Aquí está el cambio clave!
            };

            if (!id) {
                formData.Password = modalGroomerPasswordInput.value;
                if (!formData.Password) {
                    alert('La contraseña es requerida para un nuevo peluquero.');
                    return;
                }
            }

            try {
                const response = await makeApiRequest(url, method, formData);

                hideModal(groomerModal);
                alert(`Peluquero ${id ? 'actualizado' : 'registrado'} con éxito.`);
                loadGroomers();

            } catch (error) {
                console.error('Error al guardar peluquero:', error);
                alert(`Error al guardar peluquero: ${error.message}`);
            }
        });
    }

    // --- Eliminar Peluquero (DELETE) ---
    async function deleteGroomer(id) {
        if (!confirm('¿Estás seguro de que quieres eliminar este peluquero? Esto también eliminará su cuenta de usuario y no se puede deshacer.')) {
            return;
        }

        try {
            await makeApiRequest(`${API_BASE_URL}/${id}`, 'DELETE');
            alert('Peluquero eliminado con éxito.');
            loadGroomers();
        } catch (error) {
            console.error('Error al eliminar peluquero:', error);
            alert(`Error al eliminar peluquero: ${error.message}`);
        }
    }

    // --- Cerrar Modales ---
    if (closeGroomerModalButton) {
        closeGroomerModalButton.addEventListener('click', () => {
            hideModal(groomerModal);
        });
    }

    if (cancelGroomerModalButton) {
        cancelGroomerModalButton.addEventListener('click', () => {
            hideModal(groomerModal);
        });
    }

    if (groomerModal) {
        groomerModal.addEventListener('click', function (event) {
            if (event.target === groomerModal) {
                hideModal(groomerModal);
            }
        });
    }

    // --- Lógica del Modal de Calendario ---

    // Abrir Modal de Calendario
    if (configureCalendarButton) {
        configureCalendarButton.addEventListener('click', (e) => {
            e.preventDefault();
            showModal(calendarModal);
            populateCalendarForm();
        });
    }

    // Cerrar Modal de Calendario
    if (closeCalendarModalButton) {
        closeCalendarModalButton.addEventListener('click', () => {
            hideModal(calendarModal);
        });
    }

    if (calendarModal) {
        calendarModal.addEventListener('click', function (event) {
            if (event.target === calendarModal) {
                hideModal(calendarModal);
            }
        });
    }

    // Llenar el formulario de calendario con los datos de currentGroomerCalendar
    function populateCalendarForm() {
        daysOfWeek.forEach(day => {
            const startTimeInput = document.querySelector(`.start-time-input[data-day="${day}"]`);
            const endTimeInput = document.querySelector(`.end-time-input[data-day="${day}"]`);
            const enabledCheckbox = document.querySelector(`.enable-day-checkbox[data-day="${day}"]`);

            if (startTimeInput && endTimeInput && enabledCheckbox) {
                const daySchedule = currentGroomerCalendar[day];
                if (daySchedule && daySchedule.start && daySchedule.end) {
                    startTimeInput.value = daySchedule.start;
                    endTimeInput.value = daySchedule.end;
                    enabledCheckbox.checked = true;
                    startTimeInput.disabled = false;
                    endTimeInput.disabled = false;
                } else {
                    startTimeInput.value = '';
                    endTimeInput.value = '';
                    enabledCheckbox.checked = false;
                    startTimeInput.disabled = true;
                    endTimeInput.disabled = true;
                }
            }
        });
    }

    // Guardar Horario desde el Modal de Calendario
    if (calendarForm) {
        calendarForm.addEventListener('submit', (event) => {
            event.preventDefault();

            let hasValidEntry = false;
            const tempCalendar = {}; // Usar un objeto temporal para la nueva configuración

            for (const day of daysOfWeek) {
                const startTimeInput = document.querySelector(`.start-time-input[data-day="${day}"]`);
                const endTimeInput = document.querySelector(`.end-time-input[data-day="${day}"]`);
                const enabledCheckbox = document.querySelector(`.enable-day-checkbox[data-day="${day}"]`); // Obtener el checkbox

                if (!startTimeInput || !endTimeInput || !enabledCheckbox) {
                    alert('Error: Elementos de horario no encontrados para un día. Recargue la página.');
                    return;
                }

                if (enabledCheckbox.checked) { // Solo procesar si el día está habilitado
                    const start = startTimeInput.value;
                    const end = endTimeInput.value;

                    if (start && end) {
                        if (start >= end) {
                            alert(`El horario de finalización para ${day} (${end}) debe ser posterior al horario de inicio (${start}).`);
                            return;
                        }
                        tempCalendar[day] = { start, end };
                        hasValidEntry = true;
                    } else {
                        // Si el checkbox está marcado pero faltan horas, es un error
                        alert(`Para ${day}, si el día está habilitado, debe completar ambos campos de horario (inicio y fin).`);
                        return;
                    }
                } else {
                    // Si el día no está habilitado, se registra como vacío (día libre)
                    tempCalendar[day] = { start: '', end: '' };
                }
            }

            if (!hasValidEntry) {
                alert('Debe configurar al menos un día de trabajo con horarios válidos.');
                return;
            }

            currentGroomerCalendar = tempCalendar; // Actualizar el horario configurado
            hideModal(calendarModal);
            alert('Horario configurado con éxito. Recuerde guardar los cambios del peluquero.');
        });
    }

    // --- Listeners para habilitar/deshabilitar inputs de tiempo con checkboxes ---
    daysOfWeek.forEach(day => {
        const enabledCheckbox = document.querySelector(`.enable-day-checkbox[data-day="${day}"]`);
        const startTimeInput = document.querySelector(`.start-time-input[data-day="${day}"]`);
        const endTimeInput = document.querySelector(`.end-time-input[data-day="${day}"]`);

        if (enabledCheckbox && startTimeInput && endTimeInput) {
            enabledCheckbox.addEventListener('change', () => {
                const isChecked = enabledCheckbox.checked;
                startTimeInput.disabled = !isChecked;
                endTimeInput.disabled = !isChecked;

                // Limpiar valores si se deshabilita el día
                if (!isChecked) {
                    startTimeInput.value = '';
                    endTimeInput.value = '';
                }
            });
        }
    });


    // --- Cargar Peluqueros al inicio de la página (solo si estamos en la vista de peluqueros) ---
    if (groomersTableBody) {
        loadGroomers();
    }
});