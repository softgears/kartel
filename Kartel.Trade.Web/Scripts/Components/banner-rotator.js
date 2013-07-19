$(document).ready(function () {
	var el = $('.banner');
	var first = $('.banner').first().show();
	if (first) {
		var banner = first;

		if (el.length > 1) {
			var fade = 1500;
			var interval = 5000;
			$('.banner-switch[data-id="' + banner.attr('data-id') + '"]').addClass('active-switch');

			var intObj = setInterval(rotateBanner, interval);

			$('.banner-switch').click(function () {
				if ($(this).attr('data-id') != banner.attr('data-id')) {
					clearInterval(intObj);
					removeSwitch(banner);
					var id = $(this).attr('data-id');
					banner.fadeOut(fade);
					banner = $('.banner[data-id=' + id + ']');
					banner.fadeIn(fade);
					setSwitch(banner);
					intObj = setInterval(rotateBanner, interval);
				}
			});
		} else {
			$('#banners-swticher').hide();
		}
	}

	function rotateBanner() {
		removeSwitch(banner);

		banner.fadeOut(fade);
		if (banner.is(':last-child')) {
			banner = first;
		} else {
			banner = banner.next();
		}

		banner.fadeIn(fade);

		setSwitch(banner);
	}

	function setSwitch(b) {
		$('.banner-switch[data-id="' + b.attr('data-id') + '"]').addClass('active-switch');
	}

	function removeSwitch(b) {
		$('.banner-switch[data-id="' + b.attr('data-id') + '"]').removeClass('active-switch');
	}
});