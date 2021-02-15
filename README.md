# MathEventWebApp
------------------------
# DESCRIPTION
	About: web application for supporting public events
#

## AUTHENTICATION
### USERS
####        /connect/token [POST]
    POST:
        Body (x-www-form-urlencoded) (without refresh token):
            client_id: str,
            client_secret: str,
            grant_type: password credentials,
            username: str,
            password: str,
            scope: str ("offline_access" if you want to get an refresh token),
            client_authentication: send client credentials in body
        Body (x-www-form-urlencoded) (with refresh token):
            client_id: str,
            client_secret: str,
            grant_type: refresh_token,
            refresh_token: str
####        /connect/revocation [POST]
    POST:
        Body (x-www-form-urlencoded):
            token: str (access token (reference tokens only) or refresh token)
            client_id: str
            client_secret: str
####        /connect/userinfo [GET]
    GET: return the user id (Authorization = Bearer Token)

## API ENDPOINTS
### EVENTS
####        /api/Events/ [GET,POST]
    GET: return the list of events
    POST: create a new event
        Body: {
            "Name": str,
            "StartDate": datetime,
            "Hierarchy": true if the event is a series of events, otherwise null,
            "ParentId": null if the event is not part of a series of events,
            "Description": str
        } 
####         /api/Events/<int:pk>/ [GET, PUT, PATCH, DELETE]
    GET: return the event by id
    PUT: update the event
        Body: {
            "Name": str,
            "StartDate": datetime,
            "Hierarchy": true if the event is a series of events, otherwise null,
            "ParentId": null if the event is not part of a series of events,
            "Description": str,
            "ApplicationUsers": [str]
        }
    PATCH: partial update (use JsonPatchDocument)
        Fields: [
            "Name": str,
            "StartDate": datetime,
            "Hierarchy": true if the event is a series of events, otherwise null,
            "ParentId": null if the event is not part of a series of events,
            "Description": str,
            "ApplicationUsers": [str]
        ]
    DELETE: delete the event by id

### USERS
####        /api/Users/ [GET,POST]
    GET: return the list of users
    POST: create a new user
        Body: {
            "Name": str,
            "Surname": str,
            "Email": str,
            "Password": str,
            "PasswordConfirm": str
        } 
####         /api/Users/<int:pk>/ [GET, PUT, PATCH, DELETE]
    GET: return the user by id
    PUT: update the user
        Body: {
            "Name": str,
            "Surname": str,
            "Email": str,
            "Patronymic": str,
            "Events": [int]
        }
    PATCH: partial update (use JsonPatchDocument)
        Fields: [
            "Name": str,
            "Surname": str,
            "Email": str,
            "Patronymic": str,
            "Events": [int]
        ]
    DELETE: delete the user by id