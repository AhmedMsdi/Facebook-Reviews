function ThemeUsedCHanged(obj, OpCode) {
    var dd = document.getElementById("LookAndStyleDIvSurvey");
    var LogoTab = document.getElementById("LogoTab");
    if (obj) {
        LogoTab.style.display = "none";
        var UseDefaultTheme = document.getElementById("UseDefaultTheme");
        UseDefaultTheme.checked = true;
    }
    else {
        LogoTab.style.display = "block";
    }
    Survey_Name_Title_Intro_DescCHange(obj, OpCode, 'ThemeUsed');
}
function OpenColorPickerView() {
    var windowObject;
    windowObject = $("#Details").data("kendoWindow");
    windowObject.content("<div class='k-loading-mask' style='width: 100%; height: 100%; top: 0px; left: 0px;'><span class='k-loading-text'>Loading...</span><div class='k-loading-image'></div><div class='k-loading-color'></div></div>");
    windowObject.refresh({
        url: "../Surveys/ColorPicker"
    });
    windowObject.center().open();
}
function RemoveLogo(OpCode) {
    $.ajax({
        type: "POST",
        url: "/Surveys/RemoveLogo",
        data: { 'OpCode': OpCode },
        dataType: 'json',
        success: function (data) {
            if (data.Success)
                window.location.href = "/Surveys/EditSurvey?OpCode=" + OpCode;
            else alert("something seems wrong");
        },
        error: function (data) {
            alert("something seems wrong");
        }
    });
}
function TypeLook(val,OpCode) {
    var SurveyTitleDiv = document.getElementById("SurveyTitleDiv");
    SurveyTitleDiv.setAttribute("class", "SurveyTitle" + val + "Div");
    SurveyTitleDiv.setAttribute("style", "height: 50px;");
    var intro = document.getElementById("intro");
    intro.setAttribute("class", "SurveyIntro" + val + "Div");
    intro.setAttribute("style", "");
    $.ajax({
        type: "POST",
        url: "/Surveys/SaveLook",
        data: { 'OpCode': OpCode,'CSS':val },
        dataType: 'json',
        success: function (data) {
            if (data.Success) { // window.location.href = "/Surveys/EditSurvey?OpCode=" + OpCode;
            }
            else alert("something seems wrong");
        },
        error: function (data) {
            alert("something seems wrong");
        }
    });
}
function Survey_Name_Title_Intro_DescCHange(val, OpCode, FieldName) {
    if (FieldName == "Intro")
    {
        val = $("#editorIntroduction").data("kendoEditor").value($("#value").val());
        var introDIV = document.getElementById("intro");
        introDIV.innerHTML = val
    }
    if (FieldName == "Title")
    {
        var SurveyTitle = document.getElementById("SurveyTitle");
        SurveyTitle.innerHTML = val;
    }
    if (FieldName == "ThankYou")
    {
        val = $("#editorThankYouPage").data("kendoEditor").value($("#value").val());
    }
    $.ajax({
        type: "POST",
        url: "/Surveys/Survey_Name_Title_Intro_DescCHange",
        data: { 'OpCode': OpCode, 'FieldName': FieldName,'FieldValue':val},
        dataType: 'json',
        success: function (data) {
            if (data.Success) { // window.location.href = "/Surveys/EditSurvey?OpCode=" + OpCode;
            }
            else alert("something seems wrong");
        },
        error: function (data) {
            alert("something seems wrong");
        }
    });
}
function LogoPlaceCHanged(id, OpCode)
{
    Survey_Name_Title_Intro_DescCHange(id.replace('Logo', ''), OpCode, 'Logo')
}
function HideNumberingQuestioinchange(obj, opcode)
{
    Survey_Name_Title_Intro_DescCHange(obj.checked, OpCode, 'HideNBRQ')
}
function setWhitespace(val, OpCode)
{
    Survey_Name_Title_Intro_DescCHange(val, OpCode, 'WhiteSpace')
}

function paletteChange(e) {
    alert(e.value);
}