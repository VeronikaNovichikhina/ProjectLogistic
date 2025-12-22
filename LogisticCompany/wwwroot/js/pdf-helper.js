function openPrintWindow(htmlContent) {
    var newWin = window.open('', '_blank');
    newWin.document.open();
    newWin.document.write(htmlContent);

    // Добавляем кнопку печати в верх страницы
    var printButton = newWin.document.createElement("button");
    printButton.innerText = "Печать";
    printButton.style.position = "fixed";
    printButton.style.top = "10px";
    printButton.style.right = "10px";
    printButton.style.padding = "10px 20px";
    printButton.style.backgroundColor = "#28a745";
    printButton.style.color = "#fff";
    printButton.style.border = "none";
    printButton.style.cursor = "pointer";
    printButton.onclick = function () {
        newWin.print();
    };

    newWin.document.body.prepend(printButton);

    newWin.document.close();
    newWin.focus();
}





