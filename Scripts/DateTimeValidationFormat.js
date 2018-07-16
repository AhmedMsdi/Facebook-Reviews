$(document).ready(function () {
    kendo.culture("en-GB");

    $.validator.methods['date'] = function (value, element) {
        var DateAndTimeArray = value.split(" ");
        var validDate = false;
        var DateFormat = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
        var dateValue = DateAndTimeArray[0].toString();
        var stamp = value.split(" ");
        var validTime = /^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$/i.test(DateAndTimeArray[1]);
        if (DateFormat.test(dateValue)) {

            var adata = value.split('/');
            var dd = parseInt(adata[0], 10);

            var mm = parseInt(adata[1], 10);
            var yyyy = parseInt(adata[2], 10);
            var xdata = new Date(yyyy, (mm - 1), dd);

            if ((xdata.getFullYear() == yyyy) && (xdata.getMonth() == (mm - 1)) &&
                (xdata.getDate() == dd)) {

                validDate = true;
            }
            else {
                validDate = false;
            }
        } else
            validDate = false;
        return this.optional(element) || (validDate && validTime);
    }
});


function ValidateDateTime(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    var IsTimeValid= /^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$/i.test(dtValue.split(" ")[1]);
    return dtRegex.test(dtValue.split(" ")[0]) && IsTimeValid ;
}
