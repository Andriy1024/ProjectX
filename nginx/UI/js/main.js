(function(document){
    
    var description  = document.getElementsByClassName("description")[0];
    var descriptionBtn = document.getElementById("description-btn");

    descriptionBtn.onclick = function() {
        
        var element_classes = (" "+description.className+" ").replace(/[\n\t\r]/g, " "),
            remove_class    = "slide-down",
            add_class       = "slide-up",
            is_showing      = element_classes.indexOf(" "+remove_class+" ") > -1;

        if ( ! is_showing) {
            // Switch variable values
            remove_class = [add_class, add_class = remove_class][0];
            descriptionBtn.innerHTML = "Description &#129045;";
        }else{
            descriptionBtn.innerHTML = "Description &#129047;";   
        }

        // Remove the previous class (if present) and add the new class
        description.className = (element_classes.replace(" "+remove_class+" ", "") + " "+add_class+" ").trim();

        return false;
    };
    
})(document);