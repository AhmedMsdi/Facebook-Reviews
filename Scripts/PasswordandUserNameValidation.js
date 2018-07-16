

//For Password validation

function onChangeForStrength(e) {
    this.progressWrapper.css({
        "background-image": "none",
        "border-image": "none"
    });

    var Password = $("#NewPassword").val();

    var PasswordStrength = checkStrength(Password);

    this.value(PasswordStrength);

    if (PasswordStrength == 0) {
        this.progressStatus.text("Empty");
    } else if (PasswordStrength < 3) {
        this.progressStatus.text("Weak");

        this.progressWrapper.css({
            "background-color": "#EE9F05",
            "border-color": "#EE9F05"
        });
    } else if (PasswordStrength < 5) {
        this.progressStatus.text("Good");

        this.progressWrapper.css({
            "background-color": "#428bca",
            "border-color": "#428bca"
        });
    } else if (PasswordStrength <= 6) {
        this.progressStatus.text("Strong");

        this.progressWrapper.css({
            "background-color": "#8EBC00",
            "border-color": "#8EBC00"
        });
    }
}

$(document).ready(function () {
    $("#NewPassword").load('ready', function () {
        var passProgressForUpdate = $("#passStrengthForUpdatePassword").data("kendoProgressBar");
        passProgressForUpdate.progressStatus.text("Empty");
    });

    $("#NewPassword").on('input paste', function () {
        var passProgressForUpdate = $("#passStrengthForUpdatePassword").data("kendoProgressBar");
        passProgressForUpdate.value(checkStrength(this.value));
    });
    // if current password value changes then we remove the error that we've displayed
    $("#OldPassword").on('input paste', function () {
        $("#PasswordGeneralValidator")[0].innerHTML = "";
    });
});


//For UserName Validation: input and paste
function UserNameValidation() {
    var UserName = $("#NewUserName").val();
    $.ajax({
        type: "POST",
        url: "/Account/doesUserNameExist",
        data: { 'UserName': UserName },
        async:false,
        success: function (data) {
            if (data == false) {
                $("#UserNameValidation").html("User name already exists. Please enter a different user name.");
                $('#NewUserName').attr("style", "border-color: #b94a48;");
            }
            else {
                $("#UserNameValidation").html("");
                $('#NewUserName').attr("style", "border-color: #c5c5c5;");
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

function CloseUserNameWindow() {
    $("#ChangeUserNameWindow").data("kendoWindow").close();
}

function OpenChangeUserNameWindow() {
    var ChangeUserNameWindow = $("#ChangeUserNameWindow").data("kendoWindow");
    ChangeUserNameWindow.center().open();
    
    $("#NewUserName").val(FinalUserName); //this Variable is present in both Acocunt/Index and Profiles/UserProfile
    $("#UserNameValidation").empty();
    $('#NewUserName').attr("style", "border-color: #c5c5c5;");
}