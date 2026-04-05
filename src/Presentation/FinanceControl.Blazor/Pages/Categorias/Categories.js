function abrirModal() {
    document.getElementById('modalCategoria').classList.add('active');
}

function fecharModal() {
    document.getElementById('modalCategoria').classList.remove('active');
}

/* Fecha ao clicar fora do modal
document.getElementById('modalCategoria').addEventListener('click', function (e) {
    if (e.target === this) fecharModal();
});
*/