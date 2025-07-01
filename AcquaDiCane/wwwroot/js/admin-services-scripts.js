// admin-services-scripts.js

document.addEventListener('DOMContentLoaded', function () {

    // --- Configuración API ---
    const API_BASE_URL = '/api/ServicioApi';

    // --- Referencias a elementos del DOM ---
    const openAddServiceModalButton = document.getElementById('openAddServiceModalButton');
    const servicesTableBody = document.querySelector('#servicesTable tbody');

    // Elementos del Modal de Servicio (ADD/EDIT)
    const serviceModal = document.getElementById('serviceModal');
    const closeServiceModalButton = document.getElementById('closeServiceModalButton');
    const serviceModalTitle = document.getElementById('serviceModalTitle');
    const serviceForm = document.getElementById('serviceForm');
    const serviceIdInput = document.getElementById('serviceId'); // Campo oculto para el ID

    // Inputs del formulario del modal
    const modalServiceNombreInput = document.getElementById('modalServiceNombre');
    const modalServiceDescripcionInput = document.getElementById('modalServiceDescripcion');
    const modalServicePrecioInput = document.getElementById('modalServicePrecio');
    const modalServiceDuracionInput = document.getElementById('modalServiceDuracion');
    const modalServiceEstaActivoCheckbox = document.getElementById('modalServiceEstaActivo');
    const modalServiceEstaActivoGroup = document.getElementById('modalServiceEstaActivoGroup'); // Grupo para mostrar/ocultar

    const saveServiceButton = document.getElementById('saveServiceButton');
    const cancelServiceModalButton = document.getElementById('cancelServiceModalButton');

    // --- Funciones de Utilidad (Centralizadas para API y Modales) ---

    function getAuthToken() {
        // Implementa la lógica para obtener tu token JWT (ej. de localStorage)
        // return localStorage.getItem('authToken');
        return null; // Por ahora, devuelve null si no usas JWT
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

            if (response.status === 204) { // No Content
                return null;
            }

            const responseData = await response.json();

            if (!response.ok) {
                let errorMessage = `Error: ${response.status} ${response.statusText}`;
                if (responseData && typeof responseData === 'object') {
                    if (responseData.errors) {
                        const modelErrors = Object.values(responseData.errors).flat();
                        if (modelErrors.length > 0) {
                            errorMessage = modelErrors.join(', ');
                        } else if (Array.isArray(responseData)) {
                            errorMessage = responseData.join('\n');
                        }
                    } else if (responseData.message) {
                        errorMessage = responseData.message;
                    }
                } else if (typeof responseData === 'string') {
                    errorMessage = responseData;
                }
                throw new Error(errorMessage);
            }

            return responseData;
        } catch (error) {
            console.error('Error en la solicitud a la API:', error);
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
        if (!servicesTableBody) return;

        try {
            const servicios = await makeApiRequest(API_BASE_URL, 'GET');

            servicesTableBody.innerHTML = ''; // Limpiar la tabla

            if (servicios.length === 0) {
                const row = servicesTableBody.insertRow();
                const cell = row.insertCell(0);
                cell.colSpan = 6; // Nombre, Descripción, Precio, Duración, Activo, Acciones
                cell.textContent = 'No hay servicios registrados.';
                cell.style.textAlign = 'center';
                cell.style.fontStyle = 'italic';
                return;
            }

            servicios.forEach(servicio => {
                const row = servicesTableBody.insertRow();
                row.insertCell(0).textContent = servicio.nombre;
                row.insertCell(1).textContent = servicio.descripcion || 'N/A';
                row.insertCell(2).textContent = `$${servicio.precio.toFixed(2)}`;
                row.insertCell(3).textContent = `${servicio.duracionEnMinutos} min`;
                row.insertCell(4).textContent = servicio.estaActivo ? 'Sí' : 'No';

                // Celda de acciones con botones
                const actionsCell = row.insertCell(5);
                const editButton = document.createElement('button');
                editButton.classList.add('btn-info', 'edit-service-btn');
                editButton.textContent = 'Modificar';
                editButton.dataset.id = servicio.id;
                actionsCell.appendChild(editButton);

                const deleteButton = document.createElement('button');
                deleteButton.classList.add('btn-warning', 'delete-service-btn');
                deleteButton.textContent = 'Eliminar';
                deleteButton.dataset.id = servicio.id;
                actionsCell.appendChild(deleteButton);
            });

            // Añadir event listeners a los botones de editar y eliminar
            document.querySelectorAll('.edit-service-btn').forEach(button => {
                button.addEventListener('click', (e) => editService(e.target.dataset.id));
            });
            document.querySelectorAll('.delete-service-btn').forEach(button => {
                button.addEventListener('click', (e) => deleteService(e.target.dataset.id));
            });

        } catch (error) {
            console.error('Error al cargar servicios:', error);
            servicesTableBody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">Error al cargar servicios: ${error.message}</td></tr>`;
        }
    }

    // --- Abrir Modal para Agregar Servicio ---
    if (openAddServiceModalButton) {
        openAddServiceModalButton.addEventListener('click', () => {
            serviceForm.reset(); // Limpiar el formulario
            serviceIdInput.value = ''; // Asegurar que el ID esté vacío (modo creación)
            serviceModalTitle.textContent = 'Registrar Nuevo Servicio';

            modalServiceEstaActivoGroup.style.display = 'none'; // Ocultar "Está Activo" en modo creación
            modalServiceEstaActivoCheckbox.checked = true; // Por defecto activo al crear

            showModal(serviceModal);
        });
    }

    // --- Abrir Modal para Editar Servicio ---
    async function editService(id) {
        try {
            const servicio = await makeApiRequest(`${API_BASE_URL}/${id}`, 'GET');

            // Llenar el formulario con los datos del servicio
            serviceIdInput.value = servicio.id;
            modalServiceNombreInput.value = servicio.nombre;
            modalServiceDescripcionInput.value = servicio.descripcion;
            modalServicePrecioInput.value = servicio.precio;
            modalServiceDuracionInput.value = servicio.duracionEnMinutos;
            modalServiceEstaActivoCheckbox.checked = servicio.estaActivo;

            serviceModalTitle.textContent = `Editar Servicio: ${servicio.nombre}`;

            modalServiceEstaActivoGroup.style.display = 'block'; // Mostrar "Está Activo" en modo edición

            showModal(serviceModal);

        } catch (error) {
            console.error('Error al cargar datos del servicio para edición:', error);
            alert(`No se pudo cargar el servicio para editar: ${error.message}`);
        }
    }

    // --- Enviar Formulario (CREATE/UPDATE) ---
    if (serviceForm) {
        serviceForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const id = serviceIdInput.value;
            const method = id ? 'PUT' : 'POST';
            const url = id ? `${API_BASE_URL}/${id}` : API_BASE_URL;

            const formData = {
                Id: id ? parseInt(id) : 0,
                Nombre: modalServiceNombreInput.value,
                Descripcion: modalServiceDescripcionInput.value,
                Precio: parseFloat(modalServicePrecioInput.value), // Asegurarse de que sea número
                DuracionEnMinutos: parseInt(modalServiceDuracionInput.value), // Asegurarse de que sea número
                EstaActivo: id ? modalServiceEstaActivoCheckbox.checked : true, // Por defecto activo en POST
            };

            try {
                await makeApiRequest(url, method, formData);

                hideModal(serviceModal); // Ocultar el modal
                alert(`Servicio ${id ? 'actualizado' : 'registrado'} con éxito.`);
                loadServices(); // Recargar la lista de servicios

            } catch (error) {
                console.error('Error al guardar servicio:', error);
                alert(`Error al guardar servicio: ${error.message}`);
            }
        });
    }

    // --- Eliminar Servicio (DELETE) ---
    async function deleteService(id) {
        if (!confirm('¿Estás seguro de que quieres eliminar este servicio? Esto no se puede deshacer.')) {
            return;
        }

        try {
            await makeApiRequest(`${API_BASE_URL}/${id}`, 'DELETE');
            alert('Servicio eliminado con éxito.');
            loadServices(); // Recargar la lista
        } catch (error) {
            console.error('Error al eliminar servicio:', error);
            alert(`Error al eliminar servicio: ${error.message}`);
        }
    }

    // --- Cerrar Modales ---
    if (closeServiceModalButton) {
        closeServiceModalButton.addEventListener('click', () => {
            hideModal(serviceModal);
        });
    }

    if (cancelServiceModalButton) {
        cancelServiceModalButton.addEventListener('click', () => {
            hideModal(serviceModal);
        });
    }

    // Cerrar modal de servicio al hacer click fuera
    if (serviceModal) {
        serviceModal.addEventListener('click', function (event) {
            if (event.target === serviceModal) {
                hideModal(serviceModal);
            }
        });
    }

    // --- Cargar Servicios al inicio de la página (solo si estamos en la vista de servicios) ---
    if (servicesTableBody) {
        loadServices();
    }
});