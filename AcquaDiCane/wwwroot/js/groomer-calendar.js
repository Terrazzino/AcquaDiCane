document.addEventListener('DOMContentLoaded', function () {
    const groomerCalendarGrid = document.getElementById('groomerCalendarGrid');
    const currentMonthYear = document.getElementById('currentMonthYear');
    const prevMonthButton = document.getElementById('prevMonth');
    const nextMonthButton = document.getElementById('nextMonth'); // Corregido el doble "document ="
    const groomerNameSpan = document.getElementById('groomerName'); // Referencia al span del nombre del peluquero

    // Referencias a los elementos del modal principal de Turnos del Día
    const appointmentsModal = document.getElementById('appointmentsModal');
    const closeModalButton = document.getElementById('closeModalButton');
    const selectedDateDisplay = document.getElementById('selectedDateDisplay');
    const appointmentsContainer = document.getElementById('appointmentsContainer');

    // Referencias a elementos de los modales de acción
    const notifyCompletionModal = document.getElementById('notifyCompletionModal');
    const closeNotifyCompletionModalButton = document.getElementById('closeNotifyCompletionModalButton');
    const completedPetName = document.getElementById('completedPetName');
    const completedDate = document.getElementById('completedDate');
    const completedTime = document.getElementById('completedTime');
    const completionObservations = document.getElementById('completionObservations');
    const confirmCompletionButton = document.getElementById('confirmCompletionButton');

    const cancelAppointmentModal = document.getElementById('cancelAppointmentModal');
    const closeCancelAppointmentModalButton = document.getElementById('closeCancelAppointmentModalButton');
    const canceledPetName = document.getElementById('canceledPetName');
    const canceledDate = document.getElementById('canceledDate');
    const canceledTime = document.getElementById('canceledTime');
    const cancellationReason = document.getElementById('cancellationReason');
    const confirmCancellationButton = document.getElementById('confirmCancellationButton');

    let currentDate = new Date(); // La fecha actual que el calendario está mostrando

    // Variable para almacenar el ID del turno actualmente seleccionado para los modales de acción
    let currentAppointmentId = null;

    // --- FUNCIONES DE INTERACCIÓN CON LA API (REEMPLAZAN LAS DE SIMULACIÓN) ---

    // Función para obtener el nombre del peluquero logueado
    async function getLoggedInGroomerName() {
        try {
            const response = await fetch('/api/Peluquero/GetLoggedInGroomerName'); // Crea este endpoint
            if (!response.ok) {
                if (response.status === 401) {
                    // Manejar no autenticado, quizás redirigir al login
                    console.error('Usuario no autenticado para obtener nombre del peluquero.');
                    window.location.href = '/Account/Login'; // Redirige a la página de login
                    return 'Peluquero Desconocido';
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            return data.name; // Asume que la API devuelve un objeto { name: "..." }
        } catch (error) {
            console.error('Error fetching groomer name:', error);
            return 'Peluquero Desconocido'; // Nombre por defecto en caso de error
        }
    }

    // Función para obtener la disponibilidad (días con turnos/cupo) para un mes específico
    // Reemplaza getGroomerAvailability
    async function fetchGroomerAvailability(year, month) {
        try {
            // month es 0-indexado en JS, la API espera 1-indexado
            const response = await fetch(`/api/Peluquero/Availability?year=${year}&month=${month + 1}`);
            if (!response.ok) {
                if (response.status === 401) {
                    console.error('Usuario no autenticado para obtener disponibilidad del calendario.');
                    // Considera recargar la página o redirigir al login si es una acción crítica
                    return {}; // Devuelve un objeto vacío para no romper el calendario
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            // La API debe devolver un objeto o array que JS pueda mapear fácilmente a { dia: estado }
            // Por ejemplo, { "7": "available", "10": "full", "14": "available", "16": "available", "28": "available" }
            return data;
        } catch (error) {
            console.error('Error fetching groomer availability:', error);
            return {}; // Retorna un objeto vacío para manejar errores sin romper la UI
        }
    }

    // Función para obtener los turnos para una fecha específica
    // Reemplaza getAppointmentsForDate
    async function fetchAppointmentsForDate(dateString) {
        try {
            // dateString debe ser 'YYYY-MM-DD'
            const response = await fetch(`/api/Peluquero/AppointmentsByDate?date=${dateString}`);
            if (!response.ok) {
                if (response.status === 401) {
                    console.error('Usuario no autenticado para obtener turnos.');
                    return [];
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            // Asume que la API devuelve un array de objetos de turno, similar a tu `simulatedAppointments`
            return data;
        } catch (error) {
            console.error('Error fetching appointments for date:', error);
            return []; // Retorna un array vacío en caso de error
        }
    }

    // Función para notificar la finalización de un turno
    async function notifyAppointmentCompletion(appointmentId, observations) {
        try {
            const response = await fetch('/api/Peluquero/CompleteAppointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken() // Función para obtener el token CSRF
                },
                body: JSON.stringify({ appointmentId, observations })
            });

            if (!response.ok) {
                if (response.status === 401) {
                    alert('Sesión expirada o no autorizado. Por favor, inicie sesión nuevamente.');
                    window.location.href = '/Account/Login';
                    return false;
                }
                const errorData = await response.json(); // Intentar leer el error del body
                throw new Error(`Error al notificar finalización: ${errorData.message || response.statusText}`);
            }

            alert('Notificación de finalización enviada correctamente.');
            return true;
        } catch (error) {
            console.error('Error al notificar finalización:', error);
            alert(`Error: ${error.message}`);
            return false;
        }
    }

    // Función para cancelar un turno
    async function cancelGroomerAppointment(appointmentId, reason) {
        try {
            const response = await fetch('/api/Peluquero/CancelAppointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify({ appointmentId, reason })
            });

            if (!response.ok) {
                if (response.status === 401) {
                    alert('Sesión expirada o no autorizado. Por favor, inicie sesión nuevamente.');
                    window.location.href = '/Account/Login';
                    return false;
                }
                const errorData = await response.json();
                throw new Error(`Error al cancelar turno: ${errorData.message || response.statusText}`);
            }

            alert('Turno cancelado correctamente.');
            return true;
        } catch (error) {
            console.error('Error al cancelar turno:', error);
            alert(`Error: ${error.message}`);
            return false;
        }
    }

    // Función auxiliar para obtener el token anti-falsificación
    function getAntiForgeryToken() {
        // Busca el token CSRF generado por @Html.AntiForgeryToken() en tu Razor View
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : null;
    }


    // --- LÓGICA DEL CALENDARIO ---

    async function renderCalendar() {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth(); // 0-indexado

        currentMonthYear.textContent = new Date(year, month).toLocaleDateString('es-ES', { month: 'long', year: 'numeric' });
        groomerCalendarGrid.innerHTML = '';

        const dayNames = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
        dayNames.forEach(day => {
            const dayNameElement = document.createElement('div');
            dayNameElement.classList.add('calendar-day', 'day-name');
            dayNameElement.textContent = day;
            groomerCalendarGrid.appendChild(dayNameElement);
        });

        const firstDayOfMonth = new Date(year, month, 1).getDay();
        const startDayIndex = (firstDayOfMonth === 0) ? 6 : firstDayOfMonth - 1; // Ajuste para que Lun sea el primer día

        for (let i = 0; i < startDayIndex; i++) {
            const emptyDay = document.createElement('div');
            emptyDay.classList.add('calendar-day', 'empty');
            groomerCalendarGrid.appendChild(emptyDay);
        }

        const daysInMonth = new Date(year, month + 1, 0).getDate();
        // Carga la disponibilidad real de la API
        const monthAvailability = await fetchGroomerAvailability(year, month);

        for (let i = 1; i <= daysInMonth; i++) {
            const dayElement = document.createElement('button');
            dayElement.classList.add('calendar-day');
            dayElement.textContent = i;
            const formattedMonth = (month + 1).toString().padStart(2, '0');
            const formattedDay = i.toString().padStart(2, '0');
            const fullDate = `${year}-${formattedMonth}-${formattedDay}`;
            dayElement.dataset.fullDate = fullDate;

            const dayStatus = monthAvailability[i]; // Asume que la API devuelve un objeto con { día: estado }

            const hasAppointments = dayStatus !== undefined;

            if (!hasAppointments) {
                dayElement.classList.add('blocked');
                dayElement.style.cursor = 'default';
                dayElement.title = 'No hay turnos registrados para este día.';
            } else {
                dayElement.classList.remove('blocked');
                dayElement.style.cursor = 'pointer';
                dayElement.title = '';

                if (dayStatus === 'available') {
                    dayElement.classList.add('available');
                } else if (dayStatus === 'full') {
                    dayElement.classList.add('full');
                }
            }

            dayElement.addEventListener('click', async function () {
                if (this.classList.contains('blocked')) {
                    console.log(`Día ${this.dataset.fullDate} bloqueado, no hay turnos.`);
                    return;
                }
                document.querySelectorAll('.calendar-day.selected').forEach(day => day.classList.remove('selected'));
                this.classList.add('selected');
                const clickedDate = this.dataset.fullDate;
                await showAppointmentsModal(clickedDate); // Esperar a que el modal de turnos se llene
            });

            groomerCalendarGrid.appendChild(dayElement);
        }
    }

    // --- Lógica del Modal principal de Turnos del Día ---
    async function showAppointmentsModal(dateString) {
        selectedDateDisplay.textContent = new Date(dateString).toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
        appointmentsContainer.innerHTML = '<p class="loading-message">Cargando turnos...</p>'; // Mensaje de carga

        const appointments = await fetchAppointmentsForDate(dateString); // Carga los turnos reales de la API

        appointmentsContainer.innerHTML = ''; // Limpia el mensaje de carga

        if (appointments.length === 0) {
            appointmentsContainer.innerHTML = '<p class="no-appointments-message">No hay turnos registrados para esta fecha.</p>';
        } else {
            appointments.forEach(appointment => {
                const card = document.createElement('div');
                // Asume 'available-card' para el estilo de borde amarillo, u otro estilo según el estado
                // Puedes añadir lógica aquí para cambiar la clase de la tarjeta según el estado del turno (ej. "completed-card", "canceled-card")
                card.classList.add('appointment-card');
                if (appointment.status === 'Completed') {
                    card.classList.add('completed-card');
                } else if (appointment.status === 'Canceled') {
                    card.classList.add('canceled-card');
                } else {
                    card.classList.add('available-card'); // Por defecto, si está pendiente
                }


                // Asegúrate de que los campos de `appointment` coincidan con lo que tu API devolverá.
                // Por ejemplo, `appointment.petName`, `appointment.petAvatarUrl`, `appointment.startTime`, etc.
                // Adapta esto según la estructura real de tu modelo de Turno en C#.
                card.innerHTML = `
                    <div class="pet-info">
                        <img src="${appointment.petAvatarUrl || '/img/default-pet-avatar.jpg'}" alt="Foto Mascota" class="pet-avatar">
                        <div>
                            <p class="card-label">Mascota: <span class="card-value">${appointment.petName}</span></p>
                            <p class="card-label">Fecha: <span class="card-value">${new Date(appointment.date).toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' })}</span></p>
                            <p class="card-label">Horario: <span class="card-value">${appointment.time}</span></p>
                            <p class="card-label">Servicio: <span class="card-value">${appointment.serviceName}</span></p> 
                        </div>
                    </div>
                    <div class="owner-info">
                        <p class="card-label">Dueño: <span class="card-value">${appointment.ownerName}</span></p>
                        <p class="card-label">Contacto: <span class="card-value">${appointment.ownerContact}</span></p>
                        <p class="card-label">Correo: <span class="card-value">${appointment.ownerEmail}</span></p>
                    </div>
                    <div class="card-actions">
                        ${appointment.status === 'Pending' ? `
                            <button class="notify-button" data-appointment-id="${appointment.id}">Notificar Finalización</button>
                            <button class="cancel-appointment-button" data-appointment-id="${appointment.id}">Cancelar Turno</button>
                        ` : `
                            <p class="appointment-status">Estado: ${appointment.status === 'Completed' ? 'Finalizado' : 'Cancelado'}</p>
                        `}
                    </div>
                `;

                // Solo añade event listeners si el turno está pendiente
                if (appointment.status === 'Pending') {
                    card.querySelector('.notify-button').addEventListener('click', function () {
                        currentAppointmentId = appointment.id; // Guarda el ID del turno para el modal de acción
                        showNotifyCompletionModal({
                            id: appointment.id,
                            petName: appointment.petName,
                            rawDate: appointment.date, // La fecha completa en formato YYYY-MM-DD
                            time: appointment.time
                        });
                    });

                    card.querySelector('.cancel-appointment-button').addEventListener('click', function () {
                        currentAppointmentId = appointment.id; // Guarda el ID del turno para el modal de acción
                        showCancelAppointmentModal({
                            id: appointment.id,
                            petName: appointment.petName,
                            rawDate: appointment.date,
                            time: appointment.time
                        });
                    });
                }

                appointmentsContainer.appendChild(card);
            });
        }
        appointmentsModal.classList.add('active');
    }

    function hideAppointmentsModal() {
        appointmentsModal.classList.remove('active');
        document.querySelectorAll('.calendar-day.selected').forEach(day => day.classList.remove('selected'));
    }

    // --- Lógica del Modal Notificar Finalización ---
    function showNotifyCompletionModal(appointmentData) {
        completedPetName.textContent = appointmentData.petName;
        completedDate.textContent = new Date(appointmentData.rawDate).toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' });
        completedTime.textContent = appointmentData.time;
        completionObservations.value = ''; // Limpiar textarea
        notifyCompletionModal.classList.add('active');
    }

    function hideNotifyCompletionModal() {
        notifyCompletionModal.classList.remove('active');
        completionObservations.value = ''; // Asegurarse de limpiar al cerrar
    }

    // Manejador del botón Confirmar Finalización
    confirmCompletionButton.addEventListener('click', async function () {
        const observations = completionObservations.value;
        if (currentAppointmentId) {
            const success = await notifyAppointmentCompletion(currentAppointmentId, observations);
            if (success) {
                hideNotifyCompletionModal();
                // Si el modal de turnos está abierto, se vuelve a cargar para reflejar los cambios
                const selectedDay = document.querySelector('.calendar-day.selected');
                if (selectedDay) {
                    await showAppointmentsModal(selectedDay.dataset.fullDate);
                }
                renderCalendar(); // Volver a renderizar el calendario para actualizar el estado del día si el backend lo maneja
            }
        }
    });

    // --- Lógica del Modal Cancelar Turno ---
    function showCancelAppointmentModal(appointmentData) {
        canceledPetName.textContent = appointmentData.petName;
        canceledDate.textContent = new Date(appointmentData.rawDate).toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long' });
        canceledTime.textContent = appointmentData.time;
        cancellationReason.value = ''; // Limpiar textarea
        cancelAppointmentModal.classList.add('active');
    }

    function hideCancelAppointmentModal() {
        cancelAppointmentModal.classList.remove('active');
        cancellationReason.value = ''; // Asegurarse de limpiar al cerrar
    }

    // Manejador del botón Confirmar Cancelación
    confirmCancellationButton.addEventListener('click', async function () {
        const reason = cancellationReason.value;
        if (currentAppointmentId) {
            const success = await cancelGroomerAppointment(currentAppointmentId, reason);
            if (success) {
                hideCancelAppointmentModal();
                // Si el modal de turnos está abierto, se vuelve a cargar para reflejar los cambios
                const selectedDay = document.querySelector('.calendar-day.selected');
                if (selectedDay) {
                    await showAppointmentsModal(selectedDay.dataset.fullDate);
                }
                renderCalendar(); // Volver a renderizar el calendario para actualizar el estado del día
            }
        }
    });

    // --- Event Listeners Globales ---
    prevMonthButton.addEventListener('click', function () {
        currentDate.setMonth(currentDate.getMonth() - 1);
        renderCalendar();
    });

    nextMonthButton.addEventListener('click', function () {
        currentDate.setMonth(currentDate.getMonth() + 1);
        renderCalendar();
    });

    // Cerrar Modal Principal de Turnos del Día
    closeModalButton.addEventListener('click', hideAppointmentsModal);
    appointmentsModal.addEventListener('click', function (event) {
        if (event.target === appointmentsModal) { // Cierra solo si se hace clic en el overlay
            hideAppointmentsModal();
        }
    });

    // Cerrar Modal Notificar Finalización
    closeNotifyCompletionModalButton.addEventListener('click', hideNotifyCompletionModal);
    notifyCompletionModal.addEventListener('click', function (event) {
        if (event.target === notifyCompletionModal) {
            hideNotifyCompletionModal();
        }
    });

    // Cerrar Modal Cancelar Turno
    closeCancelAppointmentModalButton.addEventListener('click', hideCancelAppointmentModal);
    cancelAppointmentModal.addEventListener('click', function (event) {
        if (event.target === cancelAppointmentModal) {
            hideCancelAppointmentModal();
        }
    });

    // --- INICIALIZACIÓN ---
    async function initializeGroomerInterface() {
        const groomerName = await getLoggedInGroomerName();
        groomerNameSpan.textContent = groomerName; // Muestra el nombre del peluquero
        renderCalendar();
    }

    initializeGroomerInterface(); // Llama a la función de inicialización
});