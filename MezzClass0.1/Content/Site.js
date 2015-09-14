

$(document).on('click', '#Asignar', function () {
    var id = $(this).parents('tr').find('#ID').html();
    var nomb = $(this).parents('tr').find('#nomb').html();
    var desc = $(this).parents('tr').find('#desc').html();
    var peso = $(this).parents('tr').find('#peso').html();
    var newRow = '<tr><td hidden=""><span id="ID">' + id + '</span></td>' +
        '<td><span id="nomb">' + nomb + '</span></td>' +
          '<td><span id="desc">' + desc + '</span></td>' +
          '<td><span id="peso">' + peso + '</span></td>' +
          '<td><input type="number" id="valor" name="valorv" value="0"/></td>' +
        '<td><button type="button" class="btn btn-default btn-sm" id="deleteRow">' +
        '<span class="glyphicon glyphicon-remove-sign"></span></button></td>' +
        '</tr>';
    $(this).parents('tr').remove();
    $("#table").find('tbody').append(newRow);
    
});

function gridTojson() {
    var json = '{';
    var otArr = [];
    var cont = 0;
    var tbl2 = $('#table tbody tr').each(function (i) {
        if ($(this)[0].rowIndex != 0) {
            x = $(this).children();
            var itArr = [];

            var id = $(this).children().find("#ID").html();
            var nomb = $(this).children().find("#nomb").html();
            var nomb1 = "";
            var long = nomb.length;
            for (var i = 0; i < long; i++) {
                if (nomb[i] != ':')
                    nomb1 += nomb[i];

            }
            nomb = nomb1;
            var peso = $(this).children().find("#peso").html();
            var valor = $(this).children().find('#valor').val();
            

            itArr.push('"' + id + '"');
            itArr.push('"' + nomb + '"');
           
            itArr.push('"' + peso + '"');
            itArr.push('"' + valor + '"');
            otArr.push('"' + i + '": [' + itArr.join(',') + ']');


        }
    });
    json += otArr.join(",") + '}';
    return json;
}

$('#Crear').click(function () {
  
        var _griddata = gridTojson();
        var x = $('#jsonid');
        x[0].defaultValue = '';
        x[0].defaultValue = _griddata;
});

$('#ejemplo').click(function() {
    var c = $(this).find("#ejemplo");
    if (c.context.checked==true) {
        $('#sol').show();
       
    }
    else
    $('#sol').hide();

});

$(document).on('click', '#deleteRow', function () {
    var parentTr = $(this).parents().get(1);

  
    var id = $(this).parents('tr:first').find("#ID").html();
    var nomb = $(this).parents('tr:first').find("#nomb").html();
    var desc = $(this).parents('tr:first').find("#desc").html();

    var peso = $(this).parents('tr:first').find("#peso").html();
   
    var newRow1 = '<tr> <td> <button type="button" class="btn btn-default btn-lg" id="Asignar">' +
        '<span class="glyphicon glyphicon-download-alt"></span></button></td>' +
        '<td><span id="ID">' + id + '</span></td>' +
        '<td><span id="nomb">' + nomb + '</span></td>' +
         '<td><span id="desc">' + desc + '</span></td>' +
        '<td><span id="peso">' + peso + '</span></td>' +
        '</tr>';
    $(parentTr).remove();

    $("#Parametros").find('tbody').append(newRow1);
    var cont = $("#cont").html();
    check();
});

$(document).on('click', '#delres', function () {

    var nombre = $(this).parents('tr').find('#archivo').html();
    var tipo = $(this).parents('tr').find('#tipoarch').html();
    var $tabla = $("#deltable");
    $tabla.find("tr:gt(0)").remove();
    var newRow1 = '<tr><td><span id="archd">'+nombre+'</span></td>'+'<td><span id="ida">'+tipo +'</span></td><td> <input type="button" class="btn-danger" id="ElimR" value="Si"/></td></tr>';
    $tabla.append(newRow1);
    $('#ModemMesj').modal('toggle');
    $('#ModemMesj').modal('show');

});

$(document).on('click', '#ElimR', function () {

    var nomb = $(this).parents('tr').find('#archd').html();
    var tipo = $(this).parents('tr').find('#ida').html();
    var idp = $('#idp').html();
    var dataString = 'idp='+idp + '&nombre='+nomb;
    $.ajax({
        type: 'POST',
        url: '/TProblemas/ElimDir/',
        data: dataString,
        success: function (data) {
            if (data == 1) {
                if (tipo === "MP4" || tipo === "mp4" || tipo === "avi" || tipo === "AVI")
                    $('#video').show();
                if (tipo === "xls" || tipo === "xlsx" || tipo === "XLS" || tipo === "XLSX" || tipo === "doc" || tipo === "docx"
                    || tipo === "DOC" || tipo === "DOCX" || tipo === "pdf" || tipo === "PDF")
                    $('#documento').show();
                if (tipo === "jpg" || tipo === "jpeg" || tipo === "png" || tipo === "JPG" || tipo === "JPEG" || tipo === "PNG")
                    $('#imagen').show();

                $('#ModemMesj').modal('hide');
                $('#' + tipo).hide();
            }
           
        },
        error: function (req, stat, err) {

            alert(req.responseText);


        }
    });

  
});

$(document).on('click', '#detalles', function () {

    var id = $(this).parents('tr').find('#idprob').html();
    var dataString = 'id=' + id;
    $.ajax({
        type: 'POST',
        url: '/TProblemas/Details/',
        data: dataString,
        success: function (data) {
            var $tabla = $("#tabledet");
            $tabla.empty();
            $tabla.append(data);
            $('#modalspec').modal('show');
        },
        error: function (req, stat, err) {

            alert(req.responseText);


        }
    });
});

$(document).on('click', '#similares', function () {
    $('#imagen').show();
    var id = $(this).parents('tr').find('#idprob').html();
    var dataString = 'id=' + id;
    $.ajax({
        type: 'POST',
        url: '/Clasificar/Similares/',
        data: dataString,
        success: function (data) {
            var $tabla = $("#tabledet");
            $tabla.empty();
            $tabla.append(data);
            $('#imagen').hide();
            $('#modalspec').modal('show');
        },
        error: function (req, stat, err) {

            alert(req.responseText);


        }
    });
    
});

$(document).on('change', '#valor', function () {
  
    check();
});

$(document).on('focus', '#valor', function () {
  
    check();
});

$('#myModal').on('hidden.bs.modal', function () {
    check();
})


function check() {
    document.getElementById("Crear").disabled = "";
    $('#Crear').tooltip('destroy')

    var $vals = $('input[name=valorv]');
    if ($vals.length > 0)
    {
        for (var i = 0; i < $vals.length; i++) {
            var num = $vals.get(i).value;
            var temp = parseFloat(num);
            if (isNaN(temp) == true || temp == 0) {
                document.getElementById("Crear").disabled = "disabled";
                document.getElementById("Crear").title = "Los valores de los parametros tienen que ser numeros distintos de 0";
                $('#Crear').tooltip('toggle');
                $('#Crear').tooltip('show');
                return false;

            }
        }
    }
    
    return true;
}

//Cantidad de Niveles en 3 o menos
$(document).on('change', '#CantNiveles', function () {
    $('#alc').hide();
    $('.botoncrear').show();
    var num = $('#CantNiveles').val();
    var temp = parseFloat(num);
    if (temp >3 || temp<1) {
        $('#alc').show();
        $('.botoncrear').hide();
    }
   
});

$(document).on('click', '#enunciados', function () {
    var t1 = $('#t1').html();
    var enun1 = $('#enun').val();
    var enunp = $(this).parents('tr:first').find("#enunp").val();
    var t2 = $(this).parents('tr:first').find("#tit").html();
   
    var tabla = '<table><tr><th>Titulo</th><th>Enunciado</th></tr><tr><td>' + t1 + '</td><td>' + enun1 + '</td></tr>' +
        '<tr><td>'+t2+'</td><td>'+enunp+'</td></tr></table>';


    var $tabla = $("#tablaenunc");
    //alert(tabla);
    $tabla.empty();
    $tabla.append(tabla);
    $('#imagen').hide();
    $('#enuncmodal').modal('show');
    

});


