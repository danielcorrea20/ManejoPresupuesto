function inicializarFormularioTransacciones(urlObtenerCategorias) {
    $("TipoOperacionId").change(async function (){
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch("urlObtenerCategorias", {
            method: "post",
            body: valorSeleccionado,
            headers: {
                "content-type": "application/json"
            }
        });
        const json = await respuesta.json();
        const opciones = json.map(categoria => "<opcion value=${categoria.value}>${categoria.text}</option>");
        $("#CategoriaId").html(opciones);
    })
}