$(document).ready(function () {    

    if ($.browser.msie) {
        $('[placeholder]').focus(function () {
            var input = $(this);
            if (input.val() == input.attr('placeholder')) {
                input.val('');
                input.removeClass('placeholder');
            }
        }).blur(function () {
            var input = $(this);
            if (input.val() == '' || input.val() == input.attr('placeholder')) {
                input.addClass('placeholder');
                input.val(input.attr('placeholder'));
            }
        }).blur().parents('form').submit(function () {
            $(this).find('[placeholder]').each(function () {
                var input = $(this);
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                }
            })
        });
    }
    $("#SelectedIssueId").focus();
    $("#needHelpForm").submit(function (e) {
        if ($("#needHelpForm").valid()) {

            $.ajax({
                type: "POST",
                url: $(this).attr("action"),
                data: $(this).serialize(),
                success: function EmailSendingSuccess(status) {
                    if (status.Success) {
                        ajaxindicatorstop();
                        alert(status.Message);
                        clearForm();
                    }
                    else {
                        ajaxindicatorstop();
                        alert(status.Message);
                    }
                },
                error: function EmailSendingError() {
                    ajaxindicatorstop();                    
                    alert("Error while sending Email!");
                }
            });
            e.preventDefault();
            ajaxindicatorstart("Sending Email....", appRootPath);

        }
        else {
            e.preventDefault();
            $("#validationSummary").show();
            if ($.browser.msie) {
                $('#needHelpForm').find('[placeholder]').each(function () {
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
});
function clearForm() {
    $('#needHelpForm').find(':input').each(function () {
        switch (this.type) {
            case 'password':
            case 'select-multiple':
            case 'select-one':
            case 'text':
            case 'textarea':
            case 'tel':
            case 'email':
                $(this).val('');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
        }
    });
    $("#validationSummary").hide();
}
function ChangePwResetMsg(obj)
{
    if($(obj).val() == 6)
        $("#pwResetmsg").text('Password reset requests will be handled by AHM Support within 24 business hours.');
    else
        $("#pwResetmsg").text('');
}