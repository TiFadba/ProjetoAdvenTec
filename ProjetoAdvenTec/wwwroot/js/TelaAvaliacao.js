adicionarNovaAvaliacao = (url, idUsuario) => {
    url += "?idUsuario=" + idUsuario
    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaAvaliacao").html(res)
        }
    })
}

visualizarAvaliacao = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaAvaliacao").html(res)
        }
    })
}

editarAvaliacao = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaAvaliacao").html(res)
        }
    })
}

excluirAvaliacao = (url, idItemLista, idUsuario) => {

    url = url + "/" + idItemLista + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaAvaliacao").html(res)
        }
    })
}

voltarPaginaInicialAvaliacao = (url, idUsuario) => {

    url = url + "?idUsuario=" + idUsuario

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaAvaliacao").html(res)
        }
    })
}



// Telas de Navegação na aba de  Avaliações > Detalhes

mostrarTelaDetalhesGerais = (url, idAvaliacao, idUsuario) => {

    url = url + "/" + idAvaliacao + "?idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#nav-item-01").html(res)
        }
    })
}

mostrarTelaListaAvaliacoes = (url, idAvaliacao, idUsuario) => {

    url = url + "?idAvaliacao=" + idAvaliacao + "&idUsuario=" + idUsuario;

    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#nav-item-02").html(res)
        }
    })
}
