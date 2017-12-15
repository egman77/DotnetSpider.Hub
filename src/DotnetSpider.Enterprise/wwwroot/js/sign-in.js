$(function () {
    $('#sign_in').validate({
        highlight: function (input) {
            $(input).parents('.form-line').addClass('error');
        },
        unhighlight: function (input) {
            $(input).parents('.form-line').removeClass('error');
        },
        errorPlacement: function (error, element) {
            $(element).parents('.input-group').append(error);
        }
    });
});

function signin() {
    dsApp.post('/Account/Login', {
        email: $('#email').val(),
        password: $('#password').val(),
        rememberMe: $("#rememberme").prop("checked"),
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    }, function () {
        window.location.href = '/Home';
    });
}