function ValidardisponibilidadEmail(correo, DisponibilidadCorreo) {
    var email = document.getElementById(correo).value;
    var disponibilidadSpan = document.getElementById(DisponibilidadCorreo);
    var expresionRegularCorreo = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    var testearcorreo = expresionRegularCorreo.test(email);
    $.ajax({
        url: '/Usuario/VerificardisponibilidadEmail',
        type: "POST",
        data: { correo: email },
        success: function (result) {
            if (result.available && testearcorreo==true) {
                document.getElementById('DisponibilidadCorreo').innerText = "Correo disponible";
                disponibilidadSpan.style.color = "green";
            }
            else {
                document.getElementById('DisponibilidadCorreo').innerText = "Correo no disponible o invalido";
                disponibilidadSpan.style.color = "red";
            }
        },
        error: function () {
            alert("Error al verificar la disponibilidad del correo");
        }
    });

}

function validarformactualizar(DisponibilidadCorreo, alertvalido, Correo) {
    var disponibilidadSpan = document.getElementById(DisponibilidadCorreo);
    var alerta = document.getElementById(alertvalido);
    var email = document.getElementById(Correo).value.trim();

    if (email !== "") {
        if (disponibilidadSpan.innerText === "Correo disponible" || disponibilidadSpan.innerText === "") {
            return true;
        }
    }
    alerta.style.display = 'block';
    return false;
}