

$(document).ready(function () {

    function ValidateDate(dtValue) {
        var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
        return dtRegex.test(dtValue);
    }

    function ReverseChangeDateFormat(date) {
        var parts = date.split('/');
        var newFormat = parts[1] + "-" + parts[0] + "-" + parts[2];
        return newFormat;
    }

    $('[id^="SupR_Date_"]').each(function () {
        var id = $(this)[0].id;
        $("#" + id).keyup(function () {
            var dateValue = $("#" + id).val();
            var d = new Date(dateValue);
            var newFormatDate = ReverseChangeDateFormat(dateValue);
            var d = new Date(newFormatDate);

            if (dateValue == "") {//empty
                $("#Validator_" + id).html("");
            }
            else if (ValidateDate(dateValue) && d.toString() != "Invalid Date") {//datevalide
                $("#Validator_" + id).html("");
            }
            else {//date NON valide
                $("#Validator_" + id).html("Invalid date format (dd/MM/YYYY).");
            }

        });
    });
});

function changeInDatePicker() {  //For Interactions Tabstrip
    var Object = this._form.context;

    if (kendo.toString(this.value(), 'd') == null) {//date is not in correct Format
        // Gerer manuellement
    }
    else {
        console.log(kendo.toString());
        var jelm = $(Object);//convert to jQuery Element
        var idValue = jelm.attr("id");
        $("#Validator_" + idValue).html("");
    }
}