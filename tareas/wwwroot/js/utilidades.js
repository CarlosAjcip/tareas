//function mostrarError() {
//    alert(mensajeDeError);
//}

async function manejarErrorApi(respuesta) {
    let mensajeError = '';

    if (respuesta === 400)
    {
        mensajeError = await respuesta.text();
    } else if (respuesta.status === 404)
    {
        mensajeError = rescursoNoEncontrado;
    } else
    {
        mensajeError = rescursoInesperado;
    }
    mostrarMensajeError(mensajeError);
}

function mostrarMensajeError(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'Error...',
        text: mensaje
    })
}

function confirmarAccion({ callBackAceptar,callBackCancelar,titulo}) {
    swal.fire({
        title: titulo || 'Realmente deseas hacer esto?',
        icon: 'warning',
        showCancelButton: true,
        showCancelButtonColor: '#3085d6',
        confirmButtonColor: '#d33',
        confirmButtonText: 'Si',
        focusConfirm: true
    }).then((resultado) => {
        if (resultado.isConfirmed) {
            callBackAceptar();
        } else if (callBackCancelar) {
            //el usuario a presionando el boton de cancelar
            callBackCancelar();
        }
    })
}

//funcion par descagar archivos
function descargarArchivo(url, nombre) {
    var link = document.createElement('a');
    link.download = nombre;
    link.target = "_blank";
    link.href = url;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    delete link;
}