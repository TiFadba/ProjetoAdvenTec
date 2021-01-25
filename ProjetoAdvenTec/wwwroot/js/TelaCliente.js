adicionarNovoCliente = (url) => {
    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaCliente").html(res)
        }
    })
}

visualizarCliente = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaCliente").html(res)
        }
    })
}

editarCliente = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaCliente").html(res)
        }
    })
}

excluirCliente = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaCliente").html(res)
        }
    })
}

voltarPaginaInicialCliente = (url, idUsuario) => {

    url = url + "?idUsuario=" + idUsuario

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaCliente").html(res)
        }
    })
}