const formatedPriceInput = document.getElementById("formatedPriceInput");
const realPriceInput = document.getElementById("realPriceInput");

document.addEventListener("DOMContentLoaded", function () {
    formatInitialPrice();
});

formatedPriceInput.addEventListener('input', function () {
	let value = this.value.replace(/\D/g, '');

	if (value === '') {
		this.value = '';
		realPriceInput.value = 0;
		return;
	}

	let numericValue = parseFloat(value) / 100;

	this.value = numericValue.toLocaleString('pt-br', {
		style: 'currency',
		currency: 'BRL'
	});

	realPriceInput.value = numericValue;
});

function formatInitialPrice() {
	const initialPrice = realPriceInput.value;

	let numericValue = parseFloat(initialPrice);

	if (isNaN(numericValue)) {
		numericValue = 0;
	}
	const value = numericValue.toLocaleString('pt-br', {
		style: 'currency',
		currency: 'BRL'
	});

	formatedPriceInput.value = value;
	realPriceInput.value = numericValue
}