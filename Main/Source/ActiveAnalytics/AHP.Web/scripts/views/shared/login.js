$(document).ready(function () {
    $(window).on("load", function () {
        $("#faqs,#reports,#dvWhatsnew,#dvDashboards,#tableauDashboards").mCustomScrollbar({
            theme: "rounded-dark",
            scrollButtons: {
                enable: !0,
                scrollType: "stepped"
            }
        })
    });

    $("nav a").click(function () {
        $("nav a").removeClass("active");
        $(this).addClass("active")
    }).
        first().
        addClass("active");

    $("#IsInternalUser").click(function () {
        $(this).is(":checked") ? $("#NeedHelpLink").hide() : $("#NeedHelpLink").show()
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

    $("#UserName").focus();

    if ($("#IsInternalUser").is(":checked")) {
        $("#NeedHelpLink").hide();
    }
});