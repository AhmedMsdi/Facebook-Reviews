

function OpenProfile(mediaType, prjmkgid, opcode) {

    switch (mediaType) {
        case 'Email': {
            OpenEcastProfile(parseInt(opcode));
            break;
        }
        case 'SMS': {
            OpenSmsProfile(parseInt(prjmkgid));
            break;
        }
    }

}

function PreviewCommunication(mediaType, prjmkgid, opcode,ctcid) {
    switch (mediaType) {
        case 'Email': {
            PreviewEmail(opcode, ctcid);
            break;
        }
        case 'SMS': {
            PreviewSMS(prjmkgid);
            break;
        }
    }
}

function ReSendSMS(PrjMkgID, CtcID) {
    notification = $("#notification").data("kendoNotification");
    var kendoWindow = $("<div/>").kendoWindow({
        title: "Resend confirmation",
        resizable: false,
        modal: true
    });

    kendoWindow.data("kendoWindow")
        .content($("#resendSMS-confirmation").html())
        .center().open();

    kendoWindow
   .find(".resendSMS-confirm,.resendSMS-cancel")
       .click(function () {
           if ($(this).hasClass("resendSMS-confirm")) {

               $.ajax({
                   url: '/SMS/_ResendSMSByCtcID',
                   type: 'POST',
                   data: { 'PrjMkgID': PrjMkgID, 'CtcID': CtcID },
                   success: function (data) {
                       if (data == "Success") {
                           //udpate this grid
                           notification.show({
                               message: "SMS is resent"
                           }, "notification-success");
                       } else if (data == "InvalidContact") {
                           notification.show({
                               message: "Invalid contact"
                           }, "info");
                       }
                   },
                   error: function () {
                       alert("something seems wrong");
                   }
               });
           }
           kendoWindow.data("kendoWindow").close();
       })
       .end()
}


function ReSendEcast(EmailOpCode, CtcID) {
    notification = $("#notification").data("kendoNotification");
    var kendoWindow = $("<div id='SendConfirmation' />").kendoWindow({
        title: "Resend confirmation",
        resizable: false,
        modal: true
    });

    kendoWindow.data("kendoWindow")
     .content($("#resendEcast-confirmation").html())
     .center().open();


    kendoWindow
    .find(".resendEcast-confirm,.resendEcast-cancel")
        .click(function () {
            if ($(this).hasClass("resendEcast-confirm")) {
                $.ajax({
                    type: "POST",
                    url: "/ContactProfile/GetEcastItemIDForContact",
                    data: { 'CtcID': CtcID, 'OpCode': EmailOpCode },
                    success: function (data) {
                        if (data.split("|")[0] == "Success") {
                            var EcastItemID = data.split("|")[1];
                            $.ajax({
                                type: "POST",
                                url: "/EcastProfile/_SendMailItem",
                                data: { '_EcastItemID': EcastItemID, '_EcastOpCode': EmailOpCode },
                                dataType: 'json',
                                success: function (data) {
                                    if (data != "Error") {
                                        notification.show({
                                            message: "Email is resent"
                                        }, "notification-success");
                                    } else {
                                        notification.show({
                                            message: "Error"
                                        }, "error");
                                    }
                                }
                            });
                        } else if (data == "NotFound") {
                            notification.show({
                                message: "EcastItemID not found"
                            }, "error");
                        } else {
                            notification.show({
                                message: "Error"
                            }, "error");
                        }
                    },
                    error: function () {
                        alert("something seems wrong");
                    }
                });
            }
            kendoWindow.data("kendoWindow").close();
        })
        .end()
}