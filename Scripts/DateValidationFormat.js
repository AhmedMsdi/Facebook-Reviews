//For MVC Default validation: In order to change Date format validaiton From MM/dd/yyyy to dd/MM/yyyyy
$(document).ready(function () {
    kendo.culture("en-GB");

    $.validator.methods['date'] = function (value, element) {
        var check = false;
        var re = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
        if (re.test(value)) {

            var adata = value.split('/');
            var dd = parseInt(adata[0], 10);

            var mm = parseInt(adata[1], 10);
            var yyyy = parseInt(adata[2], 10);
            var xdata = new Date(yyyy, (mm - 1), dd);

            if ((xdata.getFullYear() == yyyy) && (xdata.getMonth() == (mm - 1)) &&
                (xdata.getDate() == dd)) {

                check = true;
            }
            else {
                check = false;
            }
        } else
            check = false;
        return this.optional(element) || check;
    }
});


//To Use in Change Event for the DatePicker(when We select a date from the datePicker) Since on date select the validation message won't be cleared until we click outside the datepicker
function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}