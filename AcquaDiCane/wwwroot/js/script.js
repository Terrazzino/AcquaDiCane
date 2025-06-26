document.addEventListener('DOMContentLoaded', function () {
    const loginButton = document.getElementById('loginButton');
    const loginForm = document.getElementById('loginForm');
    const loginFormContainer = document.getElementById('loginFormContainer');
    const leftContent = document.getElementById('leftContent');

    const loginPasswordInput = document.getElementById('loginPassword');
    // NOTA: errorMessageElement aquí está obteniendo el elemento con ID 'loginErrorMessage'
    // que probablemente ya no existe como un solo <p> sino como un <ul> de errores
    // Si estás usando `ViewData["LoginErrors"]` en <ul>, esta línea podría no encontrarlo.
    // Lo corregiremos en el IF de abajo.
    let errorMessageElement = document.getElementById('loginErrorMessage');


    const registerButton = document.getElementById('registerButton');
    const registerForm = document.getElementById('registerForm');
    const registerFormContainer = document.getElementById('registerFormContainer');
    const rightContent = document.getElementById('rightContent');

    const mainTitleOverlay = document.getElementById('mainTitleOverlay');

    // Estado inicial al cargar la página: Título principal visible, formularios ocultos
    function initializeUIState() {
        mainTitleOverlay.style.display = 'flex'; // Asegura que el título sea visible
        mainTitleOverlay.style.opacity = '1';

        loginFormContainer.classList.remove('active'); // Ocultar login
        registerFormContainer.classList.remove('active'); // Ocultar registro

        leftContent.classList.remove('inactive'); // Asegurar que los botones laterales estén visibles y activos
        rightContent.classList.remove('inactive');
        loginButton.style.display = 'block';
        registerButton.style.display = 'block';
    }


    function showFormAndHideMainTitle(formToShowContainer, contentToGrayscale, buttonToHide, otherFormContainer, otherContent, otherButton) {
        // Ocultar el título principal
        mainTitleOverlay.style.opacity = '0';
        setTimeout(() => {
            mainTitleOverlay.style.display = 'none';
        }, 500); // Coincide con la transición CSS

        // Activar el formulario deseado
        formToShowContainer.classList.add('active'); // Esto lo hará visible a través del CSS

        // Ocultar el otro formulario si está activo
        otherFormContainer.classList.remove('active');

        // Hacer inactivo el contenido que se "oscurece" (leftContent o rightContent)
        contentToGrayscale.classList.add('inactive');
        buttonToHide.style.display = 'none'; // Ocultar el botón dentro de ese contenido

        // Reactivar el otro contenido y su botón
        otherContent.classList.remove('inactive');
        otherButton.style.display = 'block';

        // Limpiar campos y errores al cambiar de formulario
        if (formToShowContainer === loginFormContainer) {
            // Limpiar registro si vamos a login
            if (registerForm) registerForm.reset();
            const registerErrorList = document.querySelector('#registerForm .error-message');
            if (registerErrorList) registerErrorList.style.display = 'none';
        } else if (formToShowContainer === registerFormContainer) {
            // Limpiar login si vamos a registro
            if (loginForm) loginForm.reset();
            const loginErrorList = document.querySelector('#loginForm .error-message');
            if (loginErrorList) loginErrorList.style.display = 'none';
            if (errorMessageElement) errorMessageElement.style.display = 'none'; // Para el error viejo de login
        }
        // Limpiar la contraseña del formulario que se está mostrando si existe
        if (loginPasswordInput) loginPasswordInput.value = '';
    }

    // Event listeners para los botones principales
    loginButton.addEventListener('click', function () {
        showFormAndHideMainTitle(loginFormContainer, leftContent, loginButton, registerFormContainer, rightContent, registerButton);
    });

    registerButton.addEventListener('click', function () {
        showFormAndHideMainTitle(registerFormContainer, rightContent, registerButton, loginFormContainer, leftContent, loginButton);
    });

    // Función para mostrar/ocultar contraseña (se mantiene igual, está correcta)
    window.togglePasswordVisibility = function (inputId) {
        const passwordInput = document.getElementById(inputId);
        const toggleButton = passwordInput.nextElementSibling;
        const showIcon = toggleButton.querySelector('.show-password-icon');
        const hideIcon = toggleButton.querySelector('.hide-password-icon');

        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            if (showIcon) showIcon.style.display = 'none';
            if (hideIcon) hideIcon.style.display = 'inline';
        } else {
            passwordInput.type = 'password';
            if (showIcon) showIcon.style.display = 'inline';
            if (hideIcon) hideIcon.style.display = 'none';
        }
    };

    // Lógica para mostrar el formulario correcto si hay errores después de un POST (al cargar la página)
    // Debes asegurarte de que `errorMessageElement` y `registerErrorList` apunten a los UL que contienen los errores
    const loginErrorList = document.querySelector('#loginForm .error-message'); // Asume que ahora es un <ul>
    const hasLoginError = loginErrorList && loginErrorList.style.display === 'block';

    const registerErrorListForCheck = document.querySelector('#registerForm .error-message');
    const hasRegisterError = registerErrorListForCheck && registerErrorListForCheck.style.display === 'block';

    if (hasLoginError) {
        showFormAndHideMainTitle(loginFormContainer, leftContent, loginButton, registerFormContainer, rightContent, registerButton);
    } else if (hasRegisterError) {
        showFormAndHideMainTitle(registerFormContainer, rightContent, registerButton, loginFormContainer, leftContent, loginButton);
    } else {
        // Si no hay errores, inicializar el estado por defecto (título visible, formularios ocultos)
        initializeUIState();
    }
});