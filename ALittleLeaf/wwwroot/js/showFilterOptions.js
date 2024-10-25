var filterOptions = document.getElementById('filter-options');
var showFilter = document.getElementById('show-filter-option');
var closeFilter = document.getElementById('close-filter-option');

showFilter.addEventListener('click', function() {
    filterOptions.classList.add('active');
    showFilter.style.display = 'none';
    closeFilter.style.display = 'block';
});

closeFilter.addEventListener('click', function() {
    filterOptions.classList.remove('active');
    showFilter.style.display = 'block';
    closeFilter.style.display = 'none';
});
