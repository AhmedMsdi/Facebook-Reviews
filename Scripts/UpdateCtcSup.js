/* Used for Add Contact From SCV and For Add Contact From Company profile*/

//IdOfDivToParse : In order to avoid conflict in SCV/CompanyProfile between Fields in add Contact and fields in Company details extra
//IsAddedNotification: if true show Added else display Updated
function UpdateCtcSupByCtcID(CtcID, IdOfDivToParse, OpenPopupContactProfile, IsAddedNotification, IsContactProfileSpecific) {
    function GetSup_R_AttributeName(attribute) {
        var index = attribute.indexOf("_", 6);
        return attribute.substring(index + 1, attribute.length);
    }

    var SupR_FieldsWithValues = "";

    //For Textboxes and TextAreas
    $('#' + IdOfDivToParse + ' [id^="SupR_TB_"]').each(function () {
        if ($(this)[0].disabled == false && $(this)[0].readOnly == false) {
            SupR_FieldsWithValues += GetSup_R_AttributeName($(this)[0].id);
            SupR_FieldsWithValues += "|**|";
            SupR_FieldsWithValues += $(this)[0].value;
            SupR_FieldsWithValues += "|**|";
        }
    });

    $('#' + IdOfDivToParse + ' [id^="SupR_CB_"]').each(function () {
        if ($(this)[0].disabled == false && $(this)[0].readOnly == false) {
            var attrName = GetSup_R_AttributeName($(this)[0].id);
            SupR_FieldsWithValues += GetSup_R_AttributeName($(this)[0].id);
            SupR_FieldsWithValues += "|**|";
            if ($('#SupR_CB_' + attrName).is(":checked"))
                SupR_FieldsWithValues += "1";
            else
                SupR_FieldsWithValues += "0";
            SupR_FieldsWithValues += "|**|";
        }
    });

    //Change format Date from dd/MM/YYYY to MM-dd-YYYY for resolving date out of range issue
    function ChangeDateFormat(date) {
        var parts = date.split('/');
        var newFormat = parts[1] + "-" + parts[0] + "-" + parts[2];
        return newFormat;
    }

    $('#' + IdOfDivToParse + ' [id^="SupR_Date_"]').each(function () {
        if ($(this)[0].disabled == false && $(this)[0].readOnly == false) {
            SupR_FieldsWithValues += GetSup_R_AttributeName($(this)[0].id);
            SupR_FieldsWithValues += "|**|";
            if ($(this)[0].value == "")
                SupR_FieldsWithValues += "";
            else
                SupR_FieldsWithValues += ChangeDateFormat($(this)[0].value);
            SupR_FieldsWithValues += "|**|"
        }
    });

    $('#' + IdOfDivToParse + ' [id^="SupR_DDL_"]').each(function () {
        if ($(this)[0].disabled == false && $(this)[0].readOnly == false) {
            SupR_FieldsWithValues += GetSup_R_AttributeName($(this)[0].id);
            SupR_FieldsWithValues += "|**|";
            SupR_FieldsWithValues += $(this)[0].value;
            SupR_FieldsWithValues += "|**|";
        }
    });

    $.ajax({
        type: "POST",
        url: "/ContactProfile/UpdateCtcSup",
        data: { 'arrayObj': SupR_FieldsWithValues, 'CtcID': CtcID },
        success: function (data) {

            if (data == "Success") {
                if (IsAddedNotification) {
                    notification.show({
                        message: "Added"
                    }, "notification-success");
                } else {
                    notification.show({
                        message: "Updated"
                    }, "notification-success");
                }
                if (IsContactProfileSpecific)//in order to remove waiting popup after update is done
                    kendo.ui.progress($("#AllContactProfileComponents"), false);
                if (OpenPopupContactProfile)
                    popup("../ContactProfile/Index?CtcID=" + CtcID);

            }
            else {
                notification.show({
                    message: "An error has occurred"
                }, "error");
            }
        },
        error: function (data) {
            alert("something seems wrong");
        }
    });
}