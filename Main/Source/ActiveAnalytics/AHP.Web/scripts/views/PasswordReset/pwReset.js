
$(document).ready(function () {
    $("#MonthYear").mask("99/9999");
    $("#personalInfoForm").submit(function (e) {
        
        if ($("#personalInfoForm").valid()) {

            $.ajax({
                type: "POST",
                url: $(this).attr("action"),
                data: $(this).serialize(),
                success: function EmailSendingSuccess(status) {
                    if (status.Success) {
                        ajaxindicatorstart(".......", appRootPath);
                        window.location.href = status.HomeUrl;
                       // clearForm();
                    }
                    else {
                        ajaxindicatorstop();
                        alert("The answers you submitted are incorrect. Please check your answers and try again. If you need assistance please contact your AHM Account Manager.");
                        
                    }
                },
                error: function EmailSendingError() {
                    ajaxindicatorstop();
                   
                    alert("error");
                }
            });
            e.preventDefault();
            ajaxindicatorstart(".....", appRootPath);

        }
        else {
            e.preventDefault();
            $("#validationSummary").show();
            if ($.browser.msie) {
                $('#personalInfoForm').find('[placeholder]').each(function () {
                    var input = $(this);
                    if (input.val() == '' || input.val() == input.attr('placeholder')) {
                        input.addClass('placeholder');
                        input.val(input.attr('placeholder'));
                        input.focus();
                    }
                });
            }
        }

    });


    $("#OldPassword, #NewPassword, #ConfirmPassword").focus(function () {
        0 == $(this).val().length && $(this).prop("type", "password").val("")
    });
    $("#OldPassword, #NewPassword, #ConfirmPassword").focusout(function () {
        0 == $(this).val().length && $(this).prop("type", "text").val("")
    });
    $.browser.msie && $("[placeholder]").focus(function () {
        var n = $(this);
        n.val() == n.attr("placeholder") && (n.val(""), n.removeClass("placeholder"))
    }).blur(function () {
        var n = $(this);
        (n.val() == "" || n.val() == n.attr("placeholder")) && (n.addClass("placeholder"), n.val(n.attr("placeholder")))
    }).blur().parents("form").submit(function () {
        $(this).find("[placeholder]").each(function () {
            var n = $(this);
            n.val() == n.attr("placeholder") && n.val("")
        })
    });
    
 
    $("#ConfirmPassword, #NewPassword").val('');


    $("#passwordResetForm").submit(function (e) {
        
        if ($("#passwordResetForm").valid()) {
            
            $.ajax({
                type: "POST",
                url: $(this).attr("action"),
                data: $(this).serialize(),
                async: false,
                success: function EmailSendingSuccess(status) {
                    if (status.Success) {
                        ajaxindicatorstart(".......", appRootPath);
                        alert('You have successfully updated your password.');
                        window.location.href = status.HomeUrl;
                        // clearForm();
                    }
                    else {
                        ajaxindicatorstop();
                        alert(status.Message);

                    }
                },
                error: function EmailSendingError() {
                    ajaxindicatorstop();
                    alert(status.Message);
                }
            });
            e.preventDefault();
        }
        else {
            
            e.preventDefault();
            $("#validationSummary").show();
            $.browser.msie && $("#passwordResetForm").find("[placeholder]").each(function () {
                var n = $(this);
                (n.val() == "" || n.val() == n.attr("placeholder")) && (n.addClass("placeholder"), n.val(n.attr("placeholder"), n.focus()))
            });
        }

    });
});