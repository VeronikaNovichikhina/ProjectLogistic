window.applyPhoneMask = (element) => {
    if (!element) return;

    element.addEventListener('input', function (e) {
        let input = e.target;
        let digits = input.value.replace(/\D/g, ''); 

        if (!digits.startsWith('7')) {
            digits = '7' + digits;
        }

        digits = digits.substring(0, 11);

        let formatted = '+';

        formatted += digits[0] || '';

        if (digits.length > 1) formatted += ' (' + digits.substring(1, 4);
        if (digits.length >= 5) formatted += ') ' + digits.substring(4, 7);
        if (digits.length >= 8) formatted += '-' + digits.substring(7, 9);
        if (digits.length >= 10) formatted += '-' + digits.substring(9, 11);

        input.value = formatted;
    });

    element.addEventListener('focus', function (e) {
        if (!e.target.value) e.target.value = '+7 ';
    });
};






