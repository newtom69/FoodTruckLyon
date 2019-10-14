function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#ImagePreview')
                .attr('src', e.target.result)
                .width(200)
                .height(200);
        };
        reader.readAsDataURL(input.files[0]);
    }
}


//Use keyup to capture user input & mouse up to catch when user is changing the value with the arrows
    $('.trailing-decimal-input').on('keyup mouseup', function(e) {

        // on keyup check for backspace & delete, to allow user to clear the input as required
        var key = e.keyCode || e.charCode;
        if (key == 8 || key == 46) {
            return false;
        };

        // get the current input value
        let correctValue = $(this).val().toString();

        //if there is no decimal places add trailing zeros
        if (correctValue.indexOf('.') === -1) {
            correctValue += '.00';
        }

        else {

            //if there is only one number after the decimal add a trailing zero
            if (correctValue.toString().split(".")[1].length === 1) {
                correctValue += '0'
            }

            //if there is more than 2 decimal places round backdown to 2
            if (correctValue.toString().split(".")[1].length > 2) {
                correctValue = parseFloat($(this).val()).toFixed(2).toString();
            }
        }

        //update the value of the input with our conditions
        $(this).val(correctValue);
    });