(function(document){
    
    sectionSlideUpDown("description-section-header", "description-section-content");
    sectionSlideUpDown("services-section-header", "services-section-content");
    sectionSlideUpDown("stack-section-header", "stack-section-content");
    sectionSlideUpDown("architecture-section-header", "architecture-section-content");
    sectionSlideUpDown("author-section-header", "author-section-content");

    let sections = document.getElementsByClassName("section");
    let arrows = document.getElementsByClassName("arrow");
    
    for(let i = 0; i < sections.length; i++)
    {
        let temp = i;
        let section = sections[temp];
        let arrow = arrows[temp];

        section.addEventListener('click', function(event){
            arrow.classList.toggle("rotated");
        });
    }

})(document);

function sectionSlideUpDown(sectionHeaderSelector, sectionContentSelector){
    
    var sectionHeader  = document.getElementById(sectionHeaderSelector);
    var sectionContent = document.getElementById(sectionContentSelector);

    sectionHeader.onclick = function() {
        if(!sectionContent.classList.contains("slide-down"))
            sectionContent.classList.add('slide-down');
        else
            sectionContent.classList.remove('slide-down');
    };
};