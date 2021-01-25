
//Funções de Validação

function verificarSeNumeros(evt) {
    evt = (evt) ? evt : event;
    var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
        ((evt.which) ? evt.which : 0));
    if (charCode < 48 || charCode > 57) {
        alert("Por favor, digite apenas números.");
        return false;
    }
    return true;
}

function verificarSeLetras(evt) {
    evt = (evt) ? evt : event;
    var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
        ((evt.which) ? evt.which : 0));
    if (charCode > 32 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
        alert("Não é permitido letras com acento ou números.");
        return false;
    }
    return true;
}

//Funções do Front 

pesquisaCampoNomeCliente = (url, idUsuario) => {



    var textoPesquisa = document.getElementById("pesquisaCliente")

    url = url + "?nomePesquisado=" + textoPesquisa.value + "&idUsuario=" + idUsuario

    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#listaPesquisaCliente").html(res)
        }
    })
}

iniciarAvaliacao = (url, idCliente, idAvaliacao) => {
    url = url + "?idCliente=" + idCliente + "&idAvaliacao=" + idAvaliacao;
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#AreaAvaliacao").html(res);
        }
    })
}

avaliar = (url, idCliente, idAvaliacao, nota) => {
    url = url + "?idCliente=" + idCliente + "&idAvaliacao=" + idAvaliacao+ "&nota=" + nota;
    //alert(url)
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#infoParticipante").html(res);
        }
    })
}

// Telas Administrativas
mostrarTelaAdministrativaCliente = (url) => {

    //alert('status')
    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaCliente").html(res)
        }
    })
}

mostrarTelaAdministrativaAvaliacao = (url) => {
    //alert('conta')
    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaAvaliacao").html(res)
        }
    })
}

mostrarTelaAdministrativaUsuario = (url) => {
    //alert('dados')
    $.ajax({
        type: "GET",
        url, url,
        success: function (res) {
            $("#AdministrativaUsuario").html(res)
        }
    })
}

