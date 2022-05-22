$(function() {
  $('.anp-export-popover').popover({
    html: true,
    placement: 'bottom',
    content: function() {
      return $('.anp-export-popover-content').html();
    }
  });
  $('.anp-export-popover').popover('show');
  
  $('.anp-export-btn').click(function() {
    $('.demo-item').delay(1000).show(300);
    $('.btn-demo').addClass('anp-export-btn-progress');
    $('.notify-bubble').delay(5000).show(500);
    setTimeout(
      $.fn.myFunction = function() {
        $('.btn-demo').removeClass('anp-export-btn-progress');
        $('.demo-item-status').text('2015-05-05 07:14').slideDown(500);
      }, 5000);
  });
  
});