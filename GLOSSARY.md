# Desktop Clock

This context describes a floating desktop clock that can show local time and additional city clocks. The glossary keeps the product language precise while the visual and platform details evolve.

## Language

**Clock**:
A configured time display for one city and its time zone.
_Avoid_: Widget, tile

**City**:
The user-facing place name attached to a clock.
_Avoid_: Location, region

**Time zone**:
The time-zone rule set used to calculate a clock's local time, represented by an IANA time-zone identifier.
_Avoid_: UTC offset, time offset

**Clock stack**:
The ordered vertical collection of clocks shown together.
_Avoid_: Clock list, column

**Date display**:
The optional calendar-date line shown with a clock.
_Avoid_: Date toggle, calendar

**Always-on-top**:
The window behavior that keeps the clock above other application windows.
_Avoid_: Pinned, sticky

**Widget mode**:
The compact, draggable desktop presentation of the clock.
_Avoid_: Desktop mode, overlay mode

**Screen-saver mode**:
A full-screen presentation that displays clock tiles while the user is away from the computer.
_Avoid_: Screensaver widget, idle mode

**Clock reading**:
The formatted local time and date produced for a clock at a particular instant.
_Avoid_: Clock value, timestamp

**Clock order**:
The user-defined vertical sequence in which clocks appear in the widget.
_Avoid_: Priority, sort order

**Timer**:
A countdown reminder measured from a duration, such as “in 20 minutes.” A Timer fires when its duration elapses.
_Avoid_: Alarm, scheduled time

**Alarm**:
A reminder scheduled for a calendar date and clock time, such as “tomorrow at 8 AM.” An Alarm may eventually repeat according to a recurrence rule.
_Avoid_: Timer, countdown

**Reminder**:
The user-facing notification produced when a Timer or Alarm becomes due. Reminder is the shared outcome, not a replacement for either scheduling concept.
_Avoid_: Timer, Alarm
