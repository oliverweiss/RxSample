###Sample app

The app is a sample UWP application using Reactive Extensions (Rx.Net) to periodically update itself.

It implements a simple MVVM pattern, and uses UWP's `x:Bind` to bind the ViewModel to the View.

It also features a unit test for the clock service showing how to use test schedulers to test time-based operation.


The clock service ticks every second, and updates the time display.

The he clocl service has also a longer tick, which is used to simulate a heartbeat polling. At every ticks, the theme is toggled from light to dark, and an async method is called to get the status. The result of the method is then used to set the color of the dot.