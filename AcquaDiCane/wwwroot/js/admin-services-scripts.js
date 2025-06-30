// wwwroot/js/admin-services-scripts.js

document.addEventListener('DOMContentLoaded', function () {

    // --- Configuración API ---
    const API_BASE_URL = '/api/ServicioApi';

    // --- Referencias a elementos del DOM para AGREGAR SERVICIO ---
    const addServiceForm = document.getElementById('addServiceForm');
    const addServiceNameInput = document.getElementById('addServiceName');
    const addServiceDescriptionInput = document.getElementById('addServiceDescription');
    const addServicePriceInput = document.getElementById('addServicePrice');
    const addServiceDurationInput = document.getElementById('addServiceDuration');
    const addServiceButton = document.getElementById('addServiceButton');

    // --- Referencias a elementos del DOM para la TABLA DE SERVICIOS ---
    const servicesTableBody = document.querySelector('#servicesTable tbody');
    const selectedServiceNameDisplay = document.getElementById('selectedServiceName');
    const modifyServiceButton = document.getElementById('modifyServiceButton');
    const deleteServiceButton = document.getElementById('deleteServiceButton');

    let selectedServiceId = null; // Para almacenar el ID del servicio seleccionado en la tabla

    // --- Referencias a elementos del DOM para el MODAL DE EDICIÓN ---
    const serviceModal = document.getElementById('serviceModal'); // El overlay/modal completo
    const closeServiceModalButton = document.getElementById('closeServiceModalButton');
    const editServiceForm = document.getElementById('editServiceForm'); // El formulario dentro del modal
    const modalServiceIdInput = document.getElementById('modalServiceId');
    const modalEditServiceNameInput = document.getElementById('modalEditServiceName');
    const modalEditServiceDescriptionInput = document.getElementById('modalEditServiceDescription');
    const modalEditServicePriceInput = document.getElementById('modalEditServicePrice');
    const modalEditServiceDurationInput = document.getElementById('modalEditServiceDuration');
    const modalEditServiceEstaActivoCheckbox = document.getElementById('modalEditServiceEstaActivo');
    const cancelEditServiceModalButton = document.getElementById('cancelEditServiceModalButton');

    // --- Funciones de Utilidad para API y Modales ---

    function getAuthToken() {
        // Implementa la lógica para obtener tu token JWT (ej. de localStorage)
        // Si no usas JWT, simplemente devuelve null o un string vacío.
        // return localStorage.getItem('authToken');
        return null;
    }

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

            // Manejar 204 No Content (para PUT y DELETE exitosos)
            if (response.status === 204) {
                return null;
            }

            const responseData = await response.json();

            if (!response.ok) {
                let errorMessage = `Error: ${response.status} ${response.statusText}`;
                if (responseData && typeof responseData === 'object') {
                    if (responseData.errors) {
                        // ASP.NET Core ModelState errors
                        const modelErrors = Object.values(responseData.errors).flat();
                        if (modelErrors.length > 0) {
                            errorMessage = modelErrors.join('\n'); // Unir todos los mensajes de error
                        }
                    } else if (responseData.message) {
                        errorMessage = responseData.message; // Mensaje de error general de la API
                    }
                } else if (typeof responseData === 'string') {
                    errorMessage = responseData; // Mensaje de error directo del servidor
                }
                throw new Error(errorMessage);
            }

            return responseData;
        } catch (error) {
            console.error('Error en la solicitud a la API:', error);
            // Propagar el error para que pueda ser manejado por la función que llama
            throw error;
        }
    }

    function showModal(modalElement) {
        modalElement.classList.add('active');
    }

    function hideModal(modalElement) {
        modalElement.classList.remove('active');
    }

    // --- Cargar Servicios (READ) ---
    async function loadServices() {
        if (!servicesTableBody) return; // Salir si la tabla no existe

        try {
            const servicios = await makeApiRequest(API_BASE_URL, 'GET');

            servicesTableBody.innerHTML = ''; // Limpiar la tabla existente
            selectedServiceId = null; // Reiniciar selección
            selectedServiceNameDisplay.textContent = 'Ninguno';
            modifyServiceButton.disabled = true;
            deleteServiceButton.disabled = true;

            if (servicios.length === 0) {
                const row = servicesTableBody.insertRow();
                const cell = row.insertCell(0);
                cell.colSpan = 5; // Nombre, Descripción, Precio, Duración, Activo (5 columnas)
                cell.textContent = 'No hay servicios registrados.';
                cell.style.textAlign = 'center';
                cell.style.fontStyle = 'italic';
                return;
            }

            servicios.forEach(servicio => {
                const row = servicesTableBody.insertRow();
                row.dataset.id = servicio.id; // Almacenar el ID en la fila
                row.dataset.name = servicio.nombre; // Almacenar el nombre para el display

                row.insertCell(0).textContent = servicio.nombre;
                row.insertCell(1).textContent = servicio.descripcion || 'N/A'; // Mostrar N/A si no hay descripción
                row.insertCell(2).textContent = `$${servicio.precio.toFixed(2)}`;
                row.insertCell(3).textContent = `${servicio.duracionEnMinutos} min`;
                row.insertCell(4).textContent = servicio.estaActivo ? 'Sí' : 'No';

                // Añadir evento click para seleccionar la fila
                row.addEventListener('click', () => selectServiceRow(row, servicio.id, servicio.nombre));
            });

        } catch (error) {
            console.error('Error al cargar servicios:', error);
            servicesTableBody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Error al cargar servicios: ${error.message}</td></tr>`;
        }
    }

    // --- Seleccionar fila de la tabla ---
    function selectServiceRow(row, id, name) {
        // Quitar la clase 'selected' de todas las filas
        document.querySelectorAll('#servicesTable tbody tr').forEach(r => {
            r.classList.remove('selected');
        });

        // Añadir la clase 'selected' a la fila clicada
        row.classList.add('selected');

        selectedServiceId = id;
        selectedServiceNameDisplay.textContent = name;
        modifyServiceButton.disabled = false;
        deleteServiceButton.disabled = false;
    }

    // --- AGREGAR Servicio (CREATE) ---
    if (addServiceForm) {
        addServiceForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const newService = {
                Nombre: addServiceNameInput.value,
                Descripcion: addServiceDescriptionInput.value,
                Precio: parseFloat(addServicePriceInput.value),
                DuracionEnMinutos: parseInt(addServiceDurationInput.value),
                EstaActivo: true // Por defecto, el nuevo servicio es activo
            };

            // Validaciones básicas antes de enviar
            if (!newService.Nombre || newService.Precio <= 0 || newService.DuracionEnMinutos <= 0) {
                alert('Por favor, complete todos los campos obligatorios y asegúrese de que Precio y Duración sean valores positivos.');
                return;
            }

            try {
                // El makeApiRequest ya maneja el JSON.stringify
                await makeApiRequest(API_BASE_URL, 'POST', newService);
                alert('Servicio agregado con éxito!');
                addServiceForm.reset(); // Limpiar el formulario de agregar
                loadServices(); // Recargar la tabla
            } catch (error) {
                console.error('Error al agregar servicio:', error);
                alert(`Error al agregar servicio: ${error.message}`);
            }
        });
    }

    // --- MODIFICAR Servicio (Abrir Modal - READ para EDIT) ---
    if (modifyServiceButton) {
        modifyServiceButton.addEventListener('click', async () => {
            if (!selectedServiceId) {
                alert('Por favor, selecciona un servicio para modificar.');
                return;
            }

            try {
                const serviceToEdit = await makeApiRequest(`${API_BASE_URL}/${selectedServiceId}`, 'GET');

                // Llenar el formulario del modal de edición
                modalServiceIdInput.value = serviceToEdit.id;
                modalEditServiceNameInput.value = serviceToEdit.nombre;
                modalEditServiceDescriptionInput.value = serviceToEdit.descripcion || '';
                modalEditServicePriceInput.value = serviceToEdit.precio;
                modalEditServiceDurationInput.value = serviceToEdit.duracionEnMinutos;
                modalEditServiceEstaActivoCheckbox.checked = serviceToEdit.estaActivo;

                showModal(serviceModal); // Mostrar el modal
            } catch (error) {
                console.error('Error al cargar servicio para edición:', error);
                alert(`No se pudo cargar el servicio para editar: ${error.message}`);
            }
        });
    }

    // --- GUARDAR Cambios en el Modal (UPDATE) ---
    if (editServiceForm) {
        editServiceForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const updatedService = {
                Id: parseInt(modalServiceIdInput.value),
                Nombre: modalEditServiceNameInput.value,
                Descripcion: modalEditServiceDescriptionInput.value,
                Precio: parseFloat(modalEditServicePriceInput.value),
                DuracionEnMinutos: parseInt(modalEditServiceDurationInput.value),
                EstaActivo: modalEditServiceEstaActivoCheckbox.checked
            };

            // Validaciones básicas
            if (!updatedService.Nombre || updatedService.Precio <= 0 || updatedService.DuracionEnMinutos <= 0) {
                alert('Por favor, complete todos los campos obligatorios y asegúrese de que Precio y Duración sean valores positivos.');
                return;
            }

            try {
                await makeApiRequest(`${API_BASE_URL}/${updatedService.Id}`, 'PUT', updatedService);
                alert('Servicio actualizado con éxito!');
                hideModal(serviceModal); // Ocultar el modal
                loadServices(); // Recargar la tabla
            } catch (error) {
                console.error('Error al actualizar servicio:', error);
                alert(`Error al actualizar servicio: ${error.message}`);
            }
        });
    }

    // --- ELIMINAR Servicio (DELETE) ---
    if (deleteServiceButton) {
        deleteServiceButton.addEventListener('click', async () => {
            if (!selectedServiceId) {
                alert('Por favor, selecciona un servicio para eliminar.');
                return;
            }

            if (!confirm(`¿Estás seguro de que quieres eliminar el servicio "${selectedServiceNameDisplay.textContent}"?`)) {
                return; // Si el usuario cancela, no hacemos nada
            }

            try {
                await makeApiRequest(`${API_BASE_URL}/${selectedServiceId}`, 'DELETE');
                alert('Servicio eliminado con éxito!');
                loadServices(); // Recargar la tabla
            } catch (error) {
                console.error('Error al eliminar servicio:', error);
                alert(`Error al eliminar servicio: ${error.message}`);
            }
        });
    }

    // --- Cerrar Modal de Edición ---
    if (closeServiceModalButton) {
        closeServiceModalButton.addEventListener('click', () => {
            hideModal(serviceModal);
        });
    }

    if (cancelEditServiceModalButton) {
        cancelEditServiceModalButton.addEventListener('click', () => {
            hideModal(serviceModal);
        });
    }

    // Cerrar modal al hacer click fuera (solo el overlay)
    if (serviceModal) {
        serviceModal.addEventListener('click', function (event) {
            if (event.target === serviceModal) {
                hideModal(serviceModal);
            }
        });
    }

    // --- Inicialización ---
    loadServices(); // Cargar los servicios al cargar la página
});