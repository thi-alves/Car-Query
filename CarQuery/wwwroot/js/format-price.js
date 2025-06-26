/*
	Esse script serve para formatar inputs de moeda. Ela considera dois tipos de inputs, a currencyInput (a input que deve ser formatada no formato de moeda, 
	usada para melhorar a experiência do usuário) e a numberInput (a input que deve armazenar o preço no formato de number para ser enviado ao controller 
	e ser processado).
	Para usar esse script basta importá-lo e chamar a função formatPriceInputs enviando as duas entradas que serão usadas
	formatPriceInputs(currencyInput, numberInput);
*/
function formatPriceInputs(currencyInput, numberInput) {
	formatInitialPrice(currencyInput, numberInput);

	currencyInput.addEventListener("input", function () {
		formatPriceInputEvent(currencyInput, numberInput)
	});
}
function formatPriceInputEvent(currencyInput, numberInput) {
	let value = currencyInput.value.replace(/\D/g, '');

	if (value === '') {
		currencyInput.value = '';
		numberInput.value = 0;
		return;
	}

	let numericValue = parseFloat(value) / 100;

	currencyInput.value = numericValue.toLocaleString('pt-br', {
		style: 'currency',
		currency: 'BRL'
	});

	numberInput.value = numericValue;
}

function formatInitialPrice(currencyInput, numberInput) {
	const initialPrice = numberInput.value;

	let numericValue = parseFloat(initialPrice);

	if (isNaN(numericValue)) {
		numericValue = 0;
	}
	const value = numericValue.toLocaleString('pt-br', {
		style: 'currency',
		currency: 'BRL'
	});

	currencyInput.value = value;
	numberInput.value = numericValue;
}

