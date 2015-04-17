/* poll UI component */
(function($){

    var voteMessage = 'You voted for \'{AnswerName}\' {Date}.',
   	deleteMessage = 'Delete your vote.',
	votingEndsMessage = 'Voting ends on {Date}.',
	votingEndsResultsHiddenMessage = 'Voting ends {Date}. Results will be shown at that time.',
	votingEndedMessage = 'Voting ended {Date}.',
    renderUi = function(elm, poll, readOnly, showNameDescription) {
		var canShowResults = true;
		var votingEndDate = null;
		if (poll.VotingEndDate) {
			votingEndDate = $.telligent.evolution.parseDate(poll.VotingEndDate);
			canShowResults = (new Date()) > votingEndDate || !poll.HideResultsUntilVotingComplete;
		}

    	var pollTotalVotes = 0;
    	$.each(poll.Answers, function(index, answer) { pollTotalVotes += answer.VoteCount; });
            
        var ui = $('<div></div>').addClass('polling-poll');
        if (showNameDescription) {
        	ui.append($('<div></div>').addClass('name').append($('<a></a>').attr('href', poll.Url).html(poll.Name)));
        	if (poll.Description) {
        		ui.append($('<div></div>').addClass('description').html(poll.Description));
        	}
        }
        
        var answersUi = $('<div></div>').addClass('poll-answers');
        ui.append(answersUi);
                    
        $.each(poll.Answers, function(index, answer) {
        	var percentage = pollTotalVotes > 0 ? Math.floor((answer.VoteCount / pollTotalVotes) * 100) : 0;
        	var answerUi = $('<div></div>').addClass('poll-answer').data('answerid', answer.Id).attr('title', answer.Name);
        	answerUi.append($('<div></div>').addClass('poll-name').html(answer.Name));
        	answerUi.append($('<div></div>').addClass('bar'));
        	answerUi.append($('<div></div>').addClass('graph').css('width', (percentage < 1 ? 1 : percentage) + '%'));
        	answerUi.append($('<div></div>').addClass('value').html(canShowResults ? percentage + '%' : ''));
        	
        	if (!readOnly) {
        		answerUi.css('cursor','pointer').on('click', { pollUi: elm, poll: poll, showNameDescription: showNameDescription, answer: answer}, vote);
        	}
        	
        	answersUi.append(answerUi);
        });
        
		if (votingEndDate) {
			var dateUi = $('<div></div>').addClass('poll-voting');
			var dateFormat = votingEndedMessage;
			if ((new Date()) <= votingEndDate) {
				if (poll.HideResultsUntilVotingComplete) {
					dateFormat = votingEndsResultsHiddenMessage;
				} else {
					dateFormat = votingEndsMessage;
				}
			}
			$.telligent.evolution.language.formatDateAndTime(votingEndDate, function(date) {
				dateUi.html(dateFormat.replace('{Date}', date));
			});
			ui.append(dateUi);
		}

        if (!readOnly) {
	        $.telligent.evolution.get({
	            url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/vote.json?PollId={PollId}',
	            data: {
	                PollId: poll.Id
	            },
	            cache: false,
	            dataType: 'json',
	            success: function(response) {
	            	if (response.PollVote) {
	                	$.telligent.evolution.language.formatAgoDate($.telligent.evolution.parseDate(response.PollVote.LastUpdatedDate), function(date) {
	                		var voteUi = $('<div></div>').addClass('poll-vote');
		                	voteUi.append(voteMessage.replace('{AnswerName}', response.PollVote.Answer.Name).replace('{Date}', date));
		                	voteUi.append($('<a href="#"></a>').addClass('internal-link delete-vote').html(deleteMessage).attr('title', deleteMessage).on('click', { pollUi: elm, poll: poll, showNameDescription: showNameDescription }, deleteVote));
		                	ui.append(voteUi);
		                });
	                }
	            },
	            error: function() {
	            	// fail silently
	            }
	        });
		}
        
        $(elm).fadeOut('fast', function() { 
        	$(elm).html(ui).fadeIn('fast') 
        });
    },
    vote = function(e) {
    	$.telligent.evolution.post({
            url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/vote.json',
            data: {
                PollId: e.data.poll.Id,
                PollAnswerId: e.data.answer.Id
            },
            cache: false,
            dataType: 'json',
            success: function(response) {
            	$.telligent.evolution.get({
		            url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/poll.json?Id={Id}',
		            data: {
		                Id: e.data.poll.Id
		            },
		            cache: false,
		            dataType: 'json',
		            success: function(response) {
		            	if (response.Poll) {
		                	renderUi(e.data.pollUi, response.Poll, false, e.data.showNameDescription);
		                }
		            }
		        });
            }
        });
        
        return false;
    },
   	deleteVote = function(e) {
   		$.telligent.evolution.del({
            url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/vote.json',
            data: {
                PollId: e.data.poll.Id,
            },
            cache: false,
            dataType: 'json',
            success: function(response) {
            	$.telligent.evolution.get({
		            url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/poll.json?Id={Id}',
		            data: {
		                Id: e.data.poll.Id
		            },
		            cache: false,
		            dataType: 'json',
		            success: function(response) {
		            	if (response.Poll) {
		                	renderUi(e.data.pollUi, response.Poll, false, e.data.showNameDescription);
		                }
		            }
		        });
            }
        });
        
        return false;
   	};
    
    $.telligent.evolution.ui.components.poll = {
        setup: function() {
        },
        add: function(elm, options) {
            var readOnly = options.readonly === 'true';
            var pollId = options.pollid;
            var showNameDescription = options.showname === 'true';
            
            $.telligent.evolution.get({
	            url: $.telligent.evolution.site.getBaseUrl() + 'api.ashx/v2/polls/poll.json?Id={Id}',
	            data: {
	                Id: pollId
	            },
	            cache: false,
	            dataType: 'json',
	            success: function(response) {
	            	if (response.Poll) {
	                	renderUi(elm, response.Poll, readOnly, showNameDescription);
	                }
	            },
	            error: function() {
	            	// fail silently
	            }
	        });
        },
        configure: function(options) {
        	if (options.deleteMessage != null) {
        		deleteMessage = options.deleteMessage;
        	}
        	if (options.voteMessage != null) {
        		voteMessage = options.voteMessage;
        	}
			if (options.votingEndsMessage != null) {
				votingEndsMessage = options.votingEndsMessage;
			}
			if (options.votingEndsResultsHiddenMessage != null) {
				votingEndsResultsHiddenMessage = options.votingEndsResultsHiddenMessage;
			}
			if (options.votingEndedMessage != null) {
				votingEndedMessage = options.votingEndedMessage;
			}
        }
    };

}(jQuery));
