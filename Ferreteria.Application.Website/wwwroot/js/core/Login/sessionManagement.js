//Funciones para el manejo del tiempo de la sesión.
var intervalSession;
var intervalCountDown;
var minutesCountDown;
var secondsCountDown;

function StartIntervalSession() {
    ClearIntervalsVerifySession();
    $('#backModalQuestionSession').hide();
    $('#modalQuestionSession').modal('hide');
    $('#timerModalQuestionSession').html(`0${minutesCountDown}:${secondsCountDown < 10 ? '0' + secondsCountDown : secondsCountDown}`);

    let timeoutSession = parseInt($('#timeoutSessionInMilliseconds').val());
    timeoutSession = (timeoutSession === null || isNaN(timeoutSession)) ? 1800000 : timeoutSession;

    intervalSession = setInterval(function () {
        $('#backModalQuestionSession').show();
        $('#modalQuestionSession').modal('show');

        ClearIntervalsVerifySession();
        StartIntervalCountDown();
    }, timeoutSession );
}

function StartIntervalCountDown() {
    intervalCountDown = setInterval(function () {

        secondsCountDown--;
        if (secondsCountDown === -1) {
            minutesCountDown--;
            secondsCountDown = 59;
        }

        if (minutesCountDown === -1) {
            ClearIntervalsVerifySession();

            $('#timerModalQuestionSession').html('00:00');
            //Se cierra la sesión.
            showLoading('Cerrando sesión').then(function () {
                NoReactiveSession();
            });
            return;
        }
        $('#timerModalQuestionSession').html(`0${minutesCountDown}:${secondsCountDown < 10 ? '0' + secondsCountDown : secondsCountDown}`);

    }, 1000);
}

function YesReactiveSession() {
    var isActiveLoading = $('#modal-loading').css('display') === 'block';
    var currentTextLoading = $("#modal-loading-msg").html();
    const url = $('#baseUrl').val().includes('/Load/') ? $('#baseUrl').val().replace('/Load/', '') + '/' : $('#baseUrl').val();
    
    ClearIntervalsVerifySession();
    showLoading('Activando..').then(function () {
        $.post( url + `Login/AjaxReactiveSession`, function () {
            StartIntervalSession();
            if (isActiveLoading === false) {
                hideLoading();
                StartIntervalSession();
            }
            else
                setTextLoading(currentTextLoading);

            setTimeout(function () { $('#backModalQuestionSession').hide(); $('.modal-backdrop.fade').hide(); }, 300);
        });
        
    });
}

function NoReactiveSession() {
    ClearIntervalsVerifySession();
    const url = $('#baseUrl').val().includes('/Load/') ? $('#baseUrl').val().replace('/Load/', '') + '/' : $('#baseUrl').val();
    showLoading('Cerrando sesión..').then(function () {
        $.get( url + `Login/AjaxLogOff`, function () {
            window.location.href = url + "Login"
        });
    });
}

function ClearIntervalsVerifySession() {
    if (intervalCountDown != null) {
        clearInterval(intervalCountDown);
        intervalCountDown = null;
    }
    if (intervalSession != null) {
        clearInterval(intervalSession);
        intervalSession = null;
    }

    let segundos = parseInt($('#timeWaitingForAnswerInactivitySeconds').val());
    segundos = (segundos === null || isNaN(segundos)) ? 300 : segundos;
    minutesCountDown = parseInt(((segundos) / 60));
    secondsCountDown = parseInt(segundos - (minutesCountDown * 60));  

}

function ResetTimeSession() {
    ClearIntervalsVerifySession();
    StartIntervalSession();
}

let trackModals = document.querySelector('.mouseevent');

trackModals.addEventListener('mousemove', (e) => {
    ResetTimeSession();
});

let trackContent = document.querySelector('#content');

trackContent.addEventListener('mousemove', (e) => {
    ResetTimeSession();
});


//Se inicia la verificación de la sesión.
setTimeout(function () {
        console.log('Se inició la verificación de la sesión.');
        StartIntervalSession();
}, 100);