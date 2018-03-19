$(function () {


    var primaryQuestionDdl = $("select:eq(0)"),
        secondaryQuestionDdl = $("select:eq(1)"),
        thirdQuestionDdl = $("select:eq(2)");

    primaryQuestionDdl.on("change", function (ev) {
        //iterate security questions and make sure dropdown one does not have same question as dropdown two
        if (primaryQuestionDdl.val() === secondaryQuestionDdl.val()) {
            secondaryQuestionDdl.val("Select a Question");            
        }
        if (thirdQuestionDdl.val() === primaryQuestionDdl.val()) {
            thirdQuestionDdl.val("Select a Question");
        }
    });

    secondaryQuestionDdl.on("change", function (ev) {
        //iterate security questions and make sure dropdown two does not have same question as dropdown one
        if (primaryQuestionDdl.val() === secondaryQuestionDdl.val()) {
            primaryQuestionDdl.val("Select a Question");            
        }
        if (thirdQuestionDdl.val() === secondaryQuestionDdl.val()) {
            thirdQuestionDdl.val("Select a Question");
        }
    });

    thirdQuestionDdl.on("change", function (ev) {
        //iterate security questions and make sure dropdown two does not have same question as dropdown one
        if (thirdQuestionDdl.val() === primaryQuestionDdl.val()) {
            primaryQuestionDdl.val("Select a Question");            
        }
        if (thirdQuestionDdl.val() === secondaryQuestionDdl.val()) {
            secondaryQuestionDdl.val("Select a Question");
        }
    });


});