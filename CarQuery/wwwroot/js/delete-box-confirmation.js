
//target é o atributo usado para identificar o item a ser deletado
function openConfirmBox(target) {
	document.getElementById("target").value = target;
	document.getElementById("customConfirmBox").style.display = "flex";
}

function closeConfirmBox() {
	document.getElementById("customConfirmBox").style.display = "none";
}