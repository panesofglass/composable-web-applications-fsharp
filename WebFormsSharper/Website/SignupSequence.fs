namespace Website

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html

type SignupSequence() =
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = ContactForms.SignupSequence () :> _
