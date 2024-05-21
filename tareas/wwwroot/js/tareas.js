function agregarNuevaTareaAlLitado() {
    tareaListadoViewModel.tareas.push(new tareaElementoListadoViewModel({ id: 0, titulo: '' }));
    $("[name=titulo-tarea]").last().focus();
}

async function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo();

    if (!titulo) {
        tareaListadoViewModel.tareas.pop();
        return;
    }

    const data = JSON.stringify(titulo);
    const respuesta = await fetch(urlTareas, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id);
    }
    else {
        //Mostrar mensaje de error
        manejarErrorApi(respuesta);
    }
}

//ler los datos de la base datos
async function obtenerTareas() {
    tareaListadoViewModel.cargando(true);
    const respuesta = await fetch(urlTareas, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

      if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        tareaListadoViewModel.cargando(false);
        return;
    }

    const json = await respuesta.json();
    tareaListadoViewModel.tareas([]);

    json.forEach(valor => {
        tareaListadoViewModel.tareas.push(new tareaElementoListadoViewModel(valor));
    });

    tareaListadoViewModel.cargando(false);
}

async function actualizarOrdenTareas() {
    const ids = obtenerIdsTareas();
    await enviarIdsTareasAlBackend(ids);

    const arregloOrdenado = tareaListadoViewModel.tareas.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    tareaElementoListadoViewModel.tareas([]);
    tareaElementoListadoViewModel.tareas(arregloOrdenado);s
}

function obtenerIdsTareas() {
    const ids = $("[name=titulo-tarea]").map(function(){
        return $(this).attr("data-id");
    }).get();
    return ids;
}

async function enviarIdsTareasAlBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTareas}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

async function manejarClickTarea(tarea) {
    if (tarea.esNuevo()) {
        return;
    }

    const respuesta = await fetch(`${urlTareas}/${tarea.id()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'appliaction/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();

    tareaEditarVM.id = json.id;
    tareaEditarVM.titulo(json.titulo);
    tareaEditarVM.descripcion(json.descripcion);

    modalEdtiarTareabootstrap.show()
}

async function manejarCambioEditarTarea() {
    const obj = {
        id: tareaEditarVM.id,
        titulo: tareaEditarVM.titulo(),
        descripcion: tareaEditarVM.descripcion()
    };

    if (!obj.titulo) {
        return;
    }

    await editarTareaCompleta(obj);
    const indice = tareaListadoViewModel.tareas().finIndex(x => x.id() === obj.id);
    const tarea = tareaListadoViewModel.tareas()[indice];
    tarea.titulo(obj.titulo);
}

async function editarTareaCompleta(tarea) {
    const data = JSON.stringify(tarea);

    const respuesta = await fetch(`${urlTareas}/${tarea.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        throw "error";
    }
}

function intentarBorrarTarea(tarea) {
    modalEdtiarTareabootstrap.hide();

    confirmarAccion({
        callBackAceptar: () => {
            borrarTarea(tarea);
        },
        callBackCancelar: () => {
            modalEdtiarTareabootstrap.show();
        },
        titulo: `Desea borrar la tarea ${tarea.titulo()}?`
    })
}

async function borrarTarea(tarea) {
    const idTarea = tarea.id;
    const respuesta = await fetch(`${urlTareas}/${idTarea}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    if (respuesta.ok) {
        const indice = ontenerIndiceTareaEnEdicion()
        tareaListadoViewModel.tareas.splice(indice, 1);
    }
}

function ontenerIndiceTareaEnEdicion() {
    return tareaListadoViewModel.tareas().finIndex(x => x.id() == tareaEditarVM.id);
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    })
})