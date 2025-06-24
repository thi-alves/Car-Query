/*
	Esse script é necessário para a implementação da partial view _ImageUploadWidget.cshtml, localizada em Areas/Admin/Views/Shared

	1- Antes de chamar esse script, crie uma variável denominada imagesInputName
	2- Atribua a ela o nome da variável do parâmetro do controller que deve receber a lista  de imagens
*/

let nextId = 0;
function addImage() {
	const imageInput = createImageInput();
	imageInput.click();
}

function createImageInput() {
	const imageInput = document.createElement("input");
	imageInput.type = "file";
	imageInput.style.display = "none";
	imageInput.name = imagesInputName;
	imageInput.id = "image-" + nextId;
	nextId++;

	imageInput.setAttribute("accept", "image/*");
	imageInput.addEventListener("change", () => {
		if (imageInput.files && imageInput.files.length > 0) {
			showImagePreview(imageInput);
		}
		else {
			//caso esteja vazio (usuário cancelou), excluimos a input e os elementos relacionados
			handleCancelledInput(imageInput);
		}
	});

	document.getElementById("inputs").appendChild(imageInput);
	return imageInput;
}

function showImagePreview(imageInput) {
	const previewId = "image-preview-" + imageInput.id.replace("image-", "");
	const previewExists = document.getElementById(previewId);
	if (previewExists) {
		updateImagePreview(imageInput);
	}
	else {
		createImagePreview(imageInput);
	}
}

function createImagePreview(imageInput) {
	const imageContainer = document.createElement("div");
	imageContainer.classList.add("image-container");
	imageContainer.id = "container-" + imageInput.id.replace("image-", "");

	const imagePreview = document.createElement("img");
	imagePreview.classList.add("image-preview");
	imagePreview.id = "image-preview-" + imageInput.id.replace("image-", "");

	imageContainer.appendChild(imagePreview);

	const imageOverlay = document.createElement("div");
	imageOverlay.classList.add("image-overlay");
	imageOverlay.addEventListener("click", (e) => {
		//previne da modeloverlay ser executada caso o usuário na verdade tenha apertado em remover
		if (e.target.classList.contains("remove-image-btn")) {
			return;
		}
		imageInput.click();
	})

	const text = document.createElement("p");
	text.textContent = "Alterar";

	const removeButton = document.createElement("button");
	removeButton.classList.add("remove-image-btn");
	removeButton.textContent = "Remover";
	removeButton.addEventListener("click", (e) => {
		//para previnir que também acione o click do overlay
		e.stopPropagation();
		removeImage(imageContainer, imageInput);
	});

	imageOverlay.appendChild(text);
	imageOverlay.appendChild(removeButton);

	imageContainer.appendChild(imageOverlay);

	setImageSource(imageInput, imagePreview);

	document.getElementById("preview").appendChild(imageContainer);
}

function updateImagePreview(imageInput) {
	const id = "image-preview-" + imageInput.id.replace("image-", "");
	const imagePreview = document.getElementById(id);
	setImageSource(imageInput, imagePreview);
}

function setImageSource(imageInput, imagePreview) {
	const image = imageInput.files[0];
	if (image.type.startsWith("image/")) {
		const reader = new FileReader();
		reader.onload = () => {
			imagePreview.src = reader.result;
		}
		reader.readAsDataURL(image);
	}
}

function removeImage(imageContainer, imageInput) {
	imageContainer.remove();
	imageInput.remove();
}

function handleCancelledInput(imageInput) {
	const previewId = "image-preview-" + imageInput.id.replace("image-", "");
	const existingPreview = document.getElementById(previewId);

	//exclui a preview da imagem (caso tenha sido criada)
	if (existingPreview) {
		const containerId = "container-" + imageInput.id.replace("image-", "");
		const container = document.getElementById(containerId);
		if (container) {
			container.remove();
		}
	}
	removeEmptyInput(imageInput);
}

function removeEmptyInput(imageInput) {
	if (imageInput) {
		imageInput.remove();
	}
}