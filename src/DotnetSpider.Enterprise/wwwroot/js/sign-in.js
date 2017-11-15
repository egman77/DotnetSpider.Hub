$(function () {
    $('#sign_in').validate({
        highlight: function (input) {
            console.log(input);
            $(input).parents('.form-line').addClass('error');
        },
        unhighlight: function (input) {
            $(input).parents('.form-line').removeClass('error');
        },
        errorPlacement: function (error, element) {
            $(element).parents('.input-group').append(error);
        },
        submitHandler: function (form) {
            $.ajax({
                url: '/Account/Login',
                data: {
                    email: $('#email').val(),
                    password: $('#password').val(),
                    rememberMe: $("#rememberme").prop("checked"),
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                method: 'POST',
                success: function (event) {
                    if (!event.success) {
                        swal(event.message);
                    } else {
                        if (event.returnUrl) {
                            window.location.href = event.returnUrl;
                        } else {
                            window.location.href = '/Home';
                        }
                    }
                },
                error: function () {
                    swal("无法连接到服务器!");
                }
            });
        }
    });
});