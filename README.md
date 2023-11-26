# OSM
A .NET library that wraps around the [OSM (Online Scout Manager)](https://www.onlinescoutmanager.co.uk/) API to make retrieving data simpler. I've created my own classes to represent 
the objects in OSM, as they better match my use-cases.

The OSM API is reasonably undocumented, so I'll do my best to document that here. 

## Functions

### Summary of CSA progress

A quick list of group members and which of the challenge badges they have achieved.

![csa_summary](https://github.com/mattnield/Chief-Scout-Award-Tracker/assets/11173358/4bead5ac-173b-4afb-ac1d-5943e36bfb1d)


### Detailed CSA progress

Provides a breakdown of each member against the challenge badge

![csa_detail](https://github.com/mattnield/Chief-Scout-Award-Tracker/assets/11173358/d516e2d0-5e3c-49f8-8b50-63f94b6546e7)

## Configuration

While the configuration and scopes currently support both read and write activities, none of the code in this repository performs any write 
operations.

## Scopes

_(this is taken form OSM directly - **Settings > My Account Details > Developer Tools > OAuth**)_

Prefix each of the following scopes with `section:` and add the suffix of `:read` or `:write` to determine if your application needs read 
access (`:read`) or read and write access (`:write`).

The 'administration' and 'finance' scopes have an additional suffix of `:admin` which is used for editing critical settings.

Please ask for the lowest possible set of permissions as you will not be able to see sections unless the user has all the permissions your 
application specifies.

- `administration`: Administration / settings areas.
- `attendance`: Attendance Register
- `badge`: Badge records
- `event`: Events
- `finance`: Financial areas (online payments, invoices, etc)
- `flexirecord`: Flexi-records
- `member`: Personal details, including adding/removing/transferring, emailing, obtaining contact details etc.
- `programme`: Programme
- `quartermaster`: Quartermaster's area

## Issues

- I'm really bad at CSS, so the layout could be much better!

### References

* [Newcastle Scouts - Open Source](https://opensource.newcastlescouts.org.uk/#introduction) : An unofficial Online Scout Manager
documentation. This is an (incomplete) mapping of the OSM API, as observed in HTTP requests.
