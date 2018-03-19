//Dependent on jquery
window.onload = ApplyBreadCrumb;

function ApplyBreadCrumb() {
    var $breadCrumb = $("#bread-crumb");
    if ($breadCrumb.length == 1) {
        if (window.location.hash == "#!/splash") {
            $breadCrumb.hide();
        }
        else {
            $breadCrumb.show();
        }
    }
}