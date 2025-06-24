/*
	Esse script é necessário para a implementação da partial view _AddSlide.cshtml, localizada em Areas/Admin/Views/Shared
*/

let slidesCounter = 0;
let slidesData = [];

let carsData;
let selectedCar;
let selectedImage;

var carouselTitle = document.getElementById("carouselTitle");
var slidesDataInput = document.getElementById("slidesDataInput");
var carModelInput = document.getElementById("carModel");
var suggestionsArea = document.getElementById("suggestions");
var createCarouselButton = document.getElementById("createCarouselButton");

//Essa função serve para pegar os slides existentes do Carousel e prepará-los para que sejam exibidos e possam ser gerenciados 
function initialize() {
	if (!slidesDataInput.value || slidesDataInput.value.trim() === "") {
		return;
	}
	var carouselSlides = JSON.parse(slidesDataInput.value);
	slidesDataInput.value = "";
		
	carouselSlides.forEach(carouselSlide =>{
		selectedCar = {carId: carouselSlide.CarId};
		selectedImage = {imageId: carouselSlide.Image.ImageId, imgPath: carouselSlide.Image.ImgPath};
			
		addSlide();
	});
}

function checkForm() {
	if (carouselTitle.value && slidesData.length > 0) {
		console.log("Dados completos");
		createCarouselButton.disabled = false;
	}
	else {
		console.log("Dados incompletos");
		createCarouselButton.disabled = true;
	}
}

document.addEventListener("DOMContentLoaded", function () {
	initialize();
	
	carouselTitle.addEventListener("input", checkForm);

	checkForm();
});

function openSlideForm() {
	document.getElementById("modelOverlay").style.display = "flex";
}

function closeSlideForm() {
	document.getElementById("modelOverlay").style.display = "none";
}

function search() {
	var model = carModelInput.value;

	if (model.trim() !== "") {
		fetch(`/Admin/AdminCar/SearchByBrandAndModel?model=${encodeURIComponent(model)}`)
			.then(response => {
				console.log("Status: " + response.status);

				if (!response.ok) {
					throw new Error("Erro");
				}

				return response.json();
			})
			.then(data => {
				carsData = data;

				data.forEach(car => {
					console.log("modelo: " + car.model);
				});

				showSuggestions();
			})
			.catch(error => {
				console.error("Erro na requisição: " + error);
			});
	}
}

function showSuggestions() {
	suggestionsArea.innerHTML = '';

	carsData.forEach(model => {
		var newSuggestion = document.createElement("p");
		newSuggestion.id = model.carId;
		newSuggestion.textContent = model.brand + " " + model.model;

		newSuggestion.addEventListener("click", function () {
			selectModel(this.id, this.textContent);
		});

		newSuggestion.className = "suggestion";

		suggestionsArea.appendChild(newSuggestion);
	})
}

function selectModel(id, text) {
	selectedCar = carsData.find(car => car.carId == id);

	carModelInput.value = text;

	showModelImages();
}

function showModelImages() {
	var imagesBox = document.getElementById("imagesBox");
	imagesBox.innerHTML = '';

	var title = document.createElement("h5");
	title.textContent = "Selecione uma imagem";

	imagesBox.appendChild(title);

	//Criando a div imagesArea onde será exibido as imagens
	var imagesArea = document.createElement("div");
	imagesArea.className = "images-area";
	imagesArea.id = "imagesArea";

	imagesBox.appendChild(imagesArea);

	//Configurando e imprimindo as imagens na imagesArea
	selectedCar.images.forEach(image => {
		var img = document.createElement("img");
		img.id = image.imageId;
		img.src = image.imgPath;
		img.width = 100;
		img.addEventListener("click", function () {
			applySelectedImageClass(img);
			selectedImage = image;
		})

		imagesArea.appendChild(img);
	});
}

function addSlide() {

	if (selectedCar?.carId && selectedImage?.imageId) {
		var slidesArea = document.getElementById("slidesArea");

		var slide = document.createElement("div");
		slide.className = "slide";

		var slideId = "slide-" + slidesCounter;
		slide.id = slideId;

		var img = document.createElement("img");
		img.src = selectedImage.imgPath;

		var removeBtn = document.createElement("button");
		removeBtn.type = "button";
		removeBtn.textContent = "Remover";
		removeBtn.className = "btn btn-danger";
		removeBtn.addEventListener("click", function () {
			removeSlide(slide.id);
		})

		slide.appendChild(img);
		slide.appendChild(removeBtn);

		slidesArea.appendChild(slide);

		slidesCounter++;

		var newSlide = { CarId: selectedCar.carId, ImageId: selectedImage.imageId };
		slidesData.push(newSlide);

		slidesDataInput.value = JSON.stringify(slidesData);

		carModelInput.value = "";
		document.getElementById("imagesBox").innerHTML = "";

		checkForm();
		closeSlideForm();
	}
}

function removeSlide(slideId) {
	let slideElement = document.getElementById(slideId);

	if (slideElement) {
		let slideIndex = Number(slideId.substring(6));
		slidesData.splice(slideIndex, 1);
		//Removendo o slide da tela
		slideElement.remove();

		if (slidesData.length === 0) {
			slidesCounter = 0;
			slidesDataInput.value = "";
		}
		else {

			let index = Number(slideIndex) + 1;
			let newSlideIndex = slideIndex;

			//Atualizando o id dos slides após a remoção de um slide
			for (index; index < slidesCounter; index++) {
				var nextSlideId = "slide-" + index;

				var slide = document.getElementById(nextSlideId);
				slide.id = "slide-" + newSlideIndex;

				newSlideIndex++;
			}

			slidesCounter = slidesCounter - 1;
			slidesDataInput.value = JSON.stringify(slidesData);
		}
		checkForm();
	}
	else {
		console.log("O slideId não foi identificado");
	}
}

function applySelectedImageClass(img) {
	const previousSelectedImage = document.querySelector(".selected-image");

	if (previousSelectedImage) {
		previousSelectedImage.classList.remove("selected-image");
	}

	img.classList.add("selected-image");
}