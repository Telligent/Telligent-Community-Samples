(function ($) {
	if (typeof $.telligent === 'undefined') { $.telligent = {}; }
	if (typeof $.telligent.bigsocial === 'undefined') { $.telligent.bigsocial = {}; }
	if (typeof $.telligent.bigsocial.widgets === 'undefined') { $.telligent.bigsocial.widgets = {}; }

	var _save = function (context) {
		var error = function (xhr, desc, ex) {
			$.telligent.evolution.notifications.show(desc, { type: 'error' });
			$('#' + context.wrapperId + ' a.save-poll').parent().removeClass('processing');
			$('#' + context.wrapperId + ' a.save-poll').removeClass('disabled');
		};

		var success = function (poll) {
			window.location = poll.Url;
		};

		var data = {
			GroupId: context.groupId,
			Name: $('#' + context.nameId).evolutionComposer('val'),
			Description: context.getDescription()
		};

		var endDate = $('#' + context.votingEndDateId).glowDateTimeSelector('val');
		if (endDate) {
			data.VotingEndDate = $.telligent.evolution.formatDate(endDate);
			data.HideResultsUntilVotingComplete = $('#' + context.hideResultsId).is(':checked');
		} else {
			data.ClearVotingEndDate = true;
		}

		if (context.pollId != null) {
			data.PollId = context.pollId;
			$.telligent.evolution.put({
				url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/poll.json?Id={PollId}',
				data: data,
				success: function (response) {
					_saveAnswers(context, response.Poll, function () { success(response.Poll) }, error);
				},
				error: error
			});
		} else {
			$.telligent.evolution.post({
				url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/poll.json',
				data: data,
				success: function (response) {
					_saveAnswers(context, response.Poll, function () { success(response.Poll) }, error);
				},
				error: error
			});
		}
	},
    _saveAnswers = function (context, poll, successFunction, errorFunction) {
    	var actions = [];
    	var actionIndex = -1;

    	var nextAction = function () {
    		actionIndex++;
    		if (actionIndex < actions.length) {
    			actions[actionIndex]();
    		} else {
    			successFunction();
    		}
    	}

    	var newAnswerIds = [];
    	context.answers.find('.field-item-input').each(function () {
    		var id = $(this).attr('data-pollanswerid');
    		var name = $(this).find('input').val();

    		if (id != 'NEW' && $.grep(poll.Answers, function (item, index) { return item.Id == id })) {
    			actions.push(function () {
    				$.telligent.evolution.put({
    					url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/answer.json?Id={Id}',
    					data: {
    						PollId: poll.Id,
    						Id: id,
    						Name: name
    					},
    					success: nextAction,
    					error: errorFunction
    				});
    			});

    			newAnswerIds.push(id);
    		} else {
    			actions.push(function () {
    				$.telligent.evolution.post({
    					url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/answer.json',
    					data: {
    						PollId: poll.Id,
    						Name: name
    					},
    					success: nextAction,
    					error: errorFunction
    				});
    			});
    		}
    	});

    	$.each(poll.Answers, function (index, item) {
    		if ($.inArray(item.Id, newAnswerIds) < 0) {
    			actions.push(function () {
    				$.telligent.evolution.del({
    					url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/answer.json?Id={Id}',
    					data: {
    						Id: item.Id
    					},
    					success: nextAction,
    					error: errorFunction
    				});
    			});
    		}
    	});

    	nextAction();
    },
    _attachHandlers = function (context) {
    	$.telligent.evolution.navigationConfirmation.enable();
    	var saveButton = $('#' + context.wrapperId + ' a.save-poll');
    	$.telligent.evolution.navigationConfirmation.enable();
    	$.telligent.evolution.navigationConfirmation.register(saveButton);

    	context.newAnswer.keydown(function (e) {
    		if (e.keyCode == 13) {
    			context.addAnswer.click();
    			return false;
    		}
    	});

    	context.addAnswer.click(function () {
    		var answer = $.trim(context.newAnswer.val());
    		if (answer != '') {
    			var newAnswer = $('<span class="field-item-input" data-pollanswerid="NEW"><input type="text" size="70" value="" /><a href="#" class="delete-answer" title="' + context.pollAnswerDelete + '"><span></span>' + context.pollAnswerDelete + '</a></span>');

    			newAnswer.find('input').val(answer);

    			if (context.answers.has('.field-item-input').length == 0) {
    				context.answers.html(newAnswer);
    			} else {
    				context.answers.find('.field-item-input:last').after(newAnswer);
    			}

    			context.newAnswer.val('');
    		}

    		return false;
    	});

    	$('.delete-answer', context.answers).live('click', function () {
    		if (window.confirm(context.pollAnswerDeleteConfirmation)) {
    			var item = $(this).parent();
    			item.slideUp('fast', function () { item.detach(); });
    		}

    		return false;
    	});
    },
    _addValidation = function (context) {
    	var saveButton = $('#' + context.wrapperId + ' a.save-poll');

    	saveButton.evolutionValidation({
    		validateOnLoad: context.mediaId <= 0 ? false : null,
    		onValidated: function (isValid, buttonClicked, c) {
    			if (isValid) {
    				saveButton.removeClass('disabled');
    			} else {
    				saveButton.addClass('disabled');
    			}
    		},
    		onSuccessfulClick: function (e) {
    			if (!saveButton.parent().hasClass('processing')) {
    				saveButton.parent().addClass('processing');
    				saveButton.addClass('disabled');
    				_save(context);
    			}

    			return false;
    		}
    	});

    	if ($('#' + context.nameId).length > 0) {
    		saveButton.evolutionValidation('addField', '#' + context.nameId,
                {
                	required: true,
                	messages: { required: context.nameRequiredText }
                },
                '#' + context.wrapperId + ' .field-item.name .field-item-validation',
                null
            );
    	}
    };

	$.telligent.bigsocial.widgets.createEditPoll = {
		register: function (context) {
			$('#' + context.nameId).evolutionComposer({
				plugins: ['hashtags']
			}).evolutionComposer('onkeydown', function (e) {
				if (e.which === 13) {
					return false;
				} else {
					return true;
				}
			});

			$('#' + context.votingEndDateId).glowDateTimeSelector({
				'pattern': '<1900-3000>-<01-12>-<1-31> <1-12>:<00-59> <am,pm>',
				'yearIndex': 0,
				'monthIndex': 1,
				'dayIndex': 2,
				'hourIndex': 3,
				'minuteIndex': 4,
				'amPmIndex': 5,
				'showPopup': true,
				'allowBlankvalue': true
			});

			_attachHandlers(context);
			_addValidation(context);
		}
	};
})(jQuery);