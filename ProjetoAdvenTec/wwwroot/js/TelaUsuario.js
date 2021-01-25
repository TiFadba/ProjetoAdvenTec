adicionarNovoUsuario = (url, idUsuario) => {
    url += "?idUsuario=" + idUsuario

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaUsuario").html(res)
        }
    })
}

visualizarUsuario = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaUsuario").html(res)
        }
    })
}

editarUsuario = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaUsuario").html(res)
        }
    })
}

excluirUsuario = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaUsuario").html(res)
        }
    })
}

voltarPaginaInicialUsuario = (url, idUsuario) => {

    url = url + "?idUsuario=" + idUsuario

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaUsuario").html(res)
        }
    })
}