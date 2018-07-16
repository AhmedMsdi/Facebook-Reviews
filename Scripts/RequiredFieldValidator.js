
/* Used for Manually managed validation */

//ByVal = true : For input which have .val()
//For Textbox
function RequiredFieldValidator(FieldToValidatedID, ValidatorNameID, FieldDisplayName, ByVal) {
    if (ByVal) {
        Value = $("#" + FieldToValidatedID).val();
        if (Value.length == 0) {
            $("#" + ValidatorNameID).attr("style", "color: #b94a48;");
            $("#" + ValidatorNameID).html("The " + FieldDisplayName + " field is required.");
            $('#' + FieldToValidatedID).attr("style", "border-color: #b94a48;");
            $('#' + FieldToValidatedID).focus();
            return false;
        }
        else {
            $("#" + ValidatorNameID).html("");
            $('#' + FieldToValidatedID).attr("style", "border-color: #c5c5c5;");
            return true;
        }
    }
}

//For Date
function RequiredFieldValidator_ForDate(FieldToValidatedID, ValidatorNameID, FieldDisplayName) {
    Value = $("#" + FieldToValidatedID).val();
    if (Value.length == 0) {
        $("#" + ValidatorNameID).attr("style", "color: #b94a48;");
        $("#" + ValidatorNameID).html("The " + FieldDisplayName + " field is required.");
        $('#' + FieldToValidatedID).attr("style", "border-color: #b94a48;");
        $('#' + FieldToValidatedID).focus();
        return false;
    }
    else if (!IsDateValid(Value)) {//date is not valid
        $("#" + ValidatorNameID).attr("style", "color: #b94a48;");
        $("#" + ValidatorNameID).html("The field " + FieldDisplayName + " must be a date.");
        $('#' + FieldToValidatedID).attr("style", "border-color: #b94a48;");
        return false;
    } else {//date is valid
        $("#" + ValidatorNameID).html("");
        $('#' + FieldToValidatedID).attr("style", "border-color: #c5c5c5;");
        return true;
    }
}

function ReverseChangeDateFormat(date) {
    var parts = date.split('/');
    var newFormat = parts[1] + "-" + parts[0] + "-" + parts[2];
    return newFormat;
}

function IsDateValid(dateValue) {
    var newFormatDate = ReverseChangeDateFormat(dateValue);
    var d = new Date(newFormatDate);

    if (ValidateDate(dateValue) && d.toString() != "Invalid Date") //datevalide
        return true;
    else
        return false;
}

function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}