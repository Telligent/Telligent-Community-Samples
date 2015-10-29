<?php
/*
PHP SDK for Telligent Community
*/		

/***************************************************************************
The following variable must be set for the library to communicate
with Telligent.
***************************************************************************/
define("REST_PATH",     "https://community.telligent.com/api.ashx/v2/");
define("API_KEY",	"");
define("API_USER",	"");


/***************************************************************************
* Calls a REST API and returns XML
* https://community.telligent.com/developers/w/developer85/46856.create-group-rest-endpoint
*
* path		URL to call
* postdata	Any key value pairs sent as POST
* verb		GET (default), POST, DELETE
***************************************************************************/
function CallRestApi($path, $postdata, $verb = 'GET') {
  $REST_USER_TOKEN = base64_encode ( API_KEY . ':' . API_USER );

  // Call the Telligent API
  $curl = curl_init();

  if ('GET' == $verb) {
    // Set Telligent Specific Auth Header
    curl_setopt($curl, CURLOPT_HTTPHEADER, array('Rest-User-Token: ' . $REST_USER_TOKEN));
  }
  
  if ('POST' == $verb){
    // Set Telligent Specific Auth Header
    curl_setopt($curl, CURLOPT_HTTPHEADER, array('Rest-User-Token: ' . $REST_USER_TOKEN));

    // Set method to POST and set Post Data
    curl_setopt($curl, CURLOPT_POST, 1);
    curl_setopt($curl, CURLOPT_POSTFIELDS, $postdata);
  }
  
  if ('DELETE' == $verb) {
    // Set Telligent Specific Auth Header
    curl_setopt($curl, CURLOPT_HTTPHEADER, array('Rest-User-Token: ' . $REST_USER_TOKEN,'Rest-Method: DELETE'));
    
    // Set method to POST and set Post Data
    curl_setopt($curl, CURLOPT_POST, 1);
    curl_setopt($curl, CURLOPT_POSTFIELDS, $postdata);
  }
  
  curl_setopt($curl, CURLOPT_URL, $path);
  curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);

  curl_setopt($curl, CURLOPT_VERBOSE, 1);
  curl_setopt($curl, CURLOPT_HEADER, 1);

  // Exec curl request
  $result = curl_exec($curl);

  // Get header and body
  $header_size = curl_getinfo($curl, CURLINFO_HEADER_SIZE);
  $header = substr($result, 0, $header_size);
  $body = substr($result, $header_size);

  curl_close($curl);

  return $body;
}

/***************************************************************************
* Create a new group (POST)
* https://community.telligent.com/developers/w/developer85/46856.create-group-rest-endpoint
*
* returns the ID of the newly created group
* name			Name of group to be created
* description		Description of group to be created
* grouptype 		Joinless, PublicOpen, PublicClosed, PrivateUnlisted, PrivateListed
* parentgroupid	ID of parent of this group
* autocreateapps	Automatically create a set of applications
***************************************************************************/
function CreateGroup ($name, $description, $grouptype, $parentgroupid, $autocreateapps = 'False') {
  $path = REST_PATH . 'groups.xml';
  
  $name = filter_var($name, FILTER_SANITIZE_STRING);
  $description = filter_var($description, FILTER_SANITIZE_STRING); 

  // Setup the POST NAME/VALUE pairs
  $postdata = http_build_query(
    array(
        'Name' => $name,
        'GroupType' => $grouptype,
	'Description' => $description,
	'ParentGroupId' => $parentgroupid,
	'AutoCreateApplications' => $autocreateapps
    )
  );

  // Call Rest API
  $result = CallRestApi($path, $postdata, 'POST');
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");

  // Get the Group ID
  $newGroupId = $xml->Group->Id;

  if ($newGroupId == '')
    $newGroupId = -1;

  return $newGroupId;
}

/***************************************************************************
* Delete a group (DELETE)
* https://community.telligent.com/developers/w/developer85/46857.delete-group-rest-endpoint
*
* returns the ID of the newly created group
* id					ID of group to delete
* deleteapplications		Delete all applications within the group
***************************************************************************/
function DeleteGroup ($id, $deleteapplications = 'True') {
  $path = REST_PATH . 'groups/' . $id . '.xml';

  // Setup the POST NAME/VALUE pairs
  $postdata = http_build_query(
    array(
        'DeleteApplications' => $deleteapplications
    )
  );

  // Call Rest API
  $result = CallRestApi($path, $postdata, 'DELETE');
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");

  // Success?
  $msg = $xml->Info->Message;
  if ('Group was deleted' == $msg) {
  	return 1;
  }
  
  return 0;
}

/***************************************************************************
* Get a group (GET)
* https://community.telligent.com/developers/w/developer85/46859.show-group-rest-endpoint
*
* returns the URL of the newly created group
* id					ID of group to lookup
***************************************************************************/
function GetGroup ($id) {
  $path = REST_PATH . 'groups/' . $id . '.xml';

  // Call Rest API
  $result = CallRestApi($path);
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");

  return $xml;
}

/***************************************************************************
* List sub groups (GET)
* https://community.telligent.com/developers/w/developer85/46858.list-group-rest-endpoint
*
* Returns an XML list of a group's subgroups
* parentgroupid		ID of parent to list children of
***************************************************************************/
function ListSubGroups ($parentgroupid) {
  $path = REST_PATH . 'groups/' . $parentgroupid . '/groups.xml';

  // Call Rest API
  $result = CallRestApi($path);

  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
 
  // return the XML list
  return $result;
}

/***************************************************************************
* GetGroupCountForUser (GET)
* https://community.telligent.com/developers/w/developer85/46879.list-group-user-rest-endpoint
*
* Returns a count of the groups a given user is owner of
* parentgroupid		Parent ID group must be a child of
* userid				User to return group count for
***************************************************************************/
function GetGroupCountForUser ($parentgroupid, $userid) {
  $path = REST_PATH . 'users/' . $userid . '/groups.xml';

  // Call Rest API
  $result = CallRestApi($path);

  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
  
  $count = 0;
  
  foreach ($xml->Groups->Group as $group) { 
  	if ($group->ParentGroupId == $parentgroupid) {
  		$count = $count + 1;
  	}
  }
 
  return $count;
}

/***************************************************************************
* ValidateIsSubgroup (GET)
* https://community.telligent.com/developers/w/developer85/46858.list-group-rest-endpoint
*
* Returns 1 if the group is a child of the parent group
* parentgroupid	ID of parent group 
* groupid			ID of group to validate is parent of
***************************************************************************/
function ValidateIsSubgroup ($parentgroupid, $groupid) {
    $path = REST_PATH . 'groups/' . $groupid . '.xml';

  // Call Rest API
  $result = CallRestApi($path);

  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
 
  $p = $xml->Group->ParentGroupId;

  if ($p == $parentgroupid) {
  	return 1;
  }
  
  return 0;
}

/***************************************************************************
* GetUserIdByName (GET)
* https://community.telligent.com/developers/w/developer85/46983.show-user-rest-endpoint
*
* Returns the ID of the newly created group
* username	Name of the user to lookup
***************************************************************************/
function GetUserIdByName ($username) {
  $username = filter_var($username, FILTER_SANITIZE_STRING);
  
  $path = REST_PATH . 'users/' . $username . '.xml';

  // Call Rest API
  $result = CallRestApi($path);
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
  return $xml->User->Id;
}

/***************************************************************************
* IsValidCommunityUser (GET)
* https://community.telligent.com/developers/w/developer85/46983.show-user-rest-endpoint
*
* Returns id of approved user 0 if non-existant / -1 if anything else
* email		Email of the user to looup
***************************************************************************/
function IsValidCommunityUser ($email) {
  $email = filter_var($email, FILTER_SANITIZE_STRING);
  
  $path = REST_PATH . 'users.xml?EmailAddress=' . $email;

  // Call Rest API
  $result = CallRestApi($path);
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
  
  $status = $xml->Users->User->AccountStatus;
  if ('Banned' == $status) {
  	return -1;
  }

  if ('Disapproved' == $status) {
  	return -1;
  }  
  
  if ('Approved' == $xml->Users->User->AccountStatus) {
  	return $xml->Users->User->Id;
  }
  
  return 0;
}

/***************************************************************************
* Create a group user (POST)
* https://community.telligent.com/developers/w/developer85/46877.create-group-user-rest-endpoint
*
* Makes a user an owner of a group.
* groupid			ID of group to modify
* userid			ID of user to add
* membershiptype	Owner, Manager, Member, PendingMember
***************************************************************************/
function CreateGroupUser ($groupid, $userid, $membershiptype = 'Member') {
  $path = REST_PATH . 'groups/' . $groupid . '/members/users.xml';

  // good group id value?
  if (intval($groupid) < 0) {
  	return 'Invalid group id';
  }

  // Setup the POST NAME/VALUE pairs
  $postdata = http_build_query(
    array(
        'UserId' => intval($userid),
        'GroupMembershipType' => $membershiptype
    )
  );

  // Call Rest API
  $result = CallRestApi($path, $postdata, 'POST');
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
  
  return $xml->User->MembershipType;
}

/***************************************************************************
* Create a user (POST)
* https://community.telligent.com/developers/w/developer85/46980.create-user-rest-endpoint
*
* Creates a new user in the community
* Username		Username of new user
* Password		Password of new user
* PrivateEmail		Email address of the user
***************************************************************************/
function CreateUser ($username, $password, $email) {
  $path = REST_PATH . '/users.xml';

  $username = filter_var($username, FILTER_SANITIZE_STRING);
  $password = filter_var($password, FILTER_SANITIZE_STRING);
  $email = filter_var($email, FILTER_SANITIZE_STRING);

  // Setup the POST NAME/VALUE pairs
  $postdata = http_build_query(
    array(
        'Username' => $username,
        'Password' => $password,
        'PrivateEmail' => $email
    )
  );

  // Call Rest API
  $result = CallRestApi($path, $postdata, 'POST');
  
  // Read the returned XML
  $xml = simplexml_load_string($result) or die("Error: Cannot create object");
  
  return $xml->User->Id;
}

/***************************************************************************
* ProvisionCommunity
*
* Returns the url to the new community
* parentgroupid	ID of parent to create a group under
* name			Name of the new group
* description		Description of the new group
* email			Email of user to provision
***************************************************************************/
function ProvisionCommunity ($parentgroupid, $name, $description, $email) {
	
	// First we need to ensure we have a valid user
	$userid = IsValidCommunityUser ($email);
	if($userid <= 0) {
		return -1;
	}
	
	// Create the group
	$groupid = CreateGroup ($name, $description, 'PrivateUnlisted', $parentgroupid);
	
	// Add the user to the group as an owner
	CreateGroupUser ($groupid, $userid, 'Owner');
	
	// Get the URL
	$xml = GetGroup ($groupid);
	
	return $xml->Group->Url;
}
?>