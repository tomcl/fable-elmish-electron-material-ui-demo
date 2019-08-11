module App

open System
open Elmish
open Elmish.React
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fable.MaterialUI
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props
open Fable.MaterialUI.MaterialDesignIcons
open Fable.MaterialUI.Icons
open Feliz
open Feliz.MaterialUI


let log x = Browser.Dom.console.log x


type Page =
  | Home
  | AutoComplete
  | Badges
  | Dialogs
  | SaveLoad
  | Selects
  | Snackbars
  | StaticAssets
  | TextFields
  static member All =
    [ Home; AutoComplete; Badges; Dialogs; SaveLoad; Selects;
      Snackbars; StaticAssets; TextFields ]

let pageTitle = function
  | Home -> "Home"
  | AutoComplete -> "Autocomplete"
  | Badges -> "Badges"
  | Dialogs -> "Dialogs"
  | SaveLoad -> "Save/load"
  | Selects -> "Selects"
  | StaticAssets -> "Static assets"
  | Snackbars -> "Snackbars"
  | TextFields -> "Text fields"

type Msg =
  | Navigate of Page
  | AutoCompleteMsg of AutoComplete.Msg
  | BadgesMsg of Badges.Msg
  | DialogsMsg of Dialogs.Msg
  | SaveLoadMsg of SaveLoad.Msg
  | SelectsMsg of Selects.Msg
  | SnackbarsMsg of Snackbars.Msg
  | TextFieldsMsg of TextFields.Msg

type Model =
  { Page: Page
    AutoCompleteDownshift: AutoComplete.Model
    Badges: Badges.Model
    Dialogs: Dialogs.Model
    SaveLoad: SaveLoad.Model
    Selects: Selects.Model
    Snackbars: Snackbars.Model
    TextFields: TextFields.Model }

let init () =
  let m =
    { Page = Home
      AutoCompleteDownshift = AutoComplete.init ()
      Badges = Badges.init ()
      Dialogs = Dialogs.init ()
      SaveLoad = SaveLoad.init ()
      Selects = Selects.init ()
      Snackbars = Snackbars.init ()
      TextFields = TextFields.init () }
  m, Cmd.none

let update msg m =
  match msg with
  | Navigate p ->
      { m with Page = p }, Cmd.none
  | AutoCompleteMsg msg' ->
      { m with AutoCompleteDownshift = AutoComplete.update msg' m.AutoCompleteDownshift }, Cmd.none
  | BadgesMsg msg' ->
      { m with Badges = Badges.update msg' m.Badges }, Cmd.none
  | DialogsMsg msg' ->
      { m with Dialogs = Dialogs.update msg' m.Dialogs }, Cmd.none
  | SaveLoadMsg msg' ->
      let m', cmd = SaveLoad.update msg' m.SaveLoad
      { m with SaveLoad = m' }, Cmd.map SaveLoadMsg cmd
  | SelectsMsg msg' ->
      { m with Selects = Selects.update msg' m.Selects }, Cmd.none
  | SnackbarsMsg msg' ->
      let m', cmd = Snackbars.update msg' m.Snackbars
      { m with Snackbars = m' }, Cmd.map SnackbarsMsg cmd
  | TextFieldsMsg msg' ->
      { m with TextFields = TextFields.update msg' m.TextFields }, Cmd.none


// Domain/Elmish above, view below


let private useStyles = Styles.makeStyles(fun theme ->
  let drawerWidth = 240
  {|
    root = asClassName [
      style.display.flex
    ]
    appBar = asClassName [
      style.zIndex (theme?zIndex?drawer + 1)
    ]
    drawer = asClassName [
      style.width (length.px drawerWidth)
      style.flexShrink 0
    ]
    drawerPaper = asClassName [
      style.width (length.px drawerWidth)
    ]
    content = asClassName [
      style.flexGrow 1
      style.padding (theme?spacing?unit * 3)
    ]
    toolbar = asClassName [
      yield! style.spread theme?mixins?toolbar
    ]
  |}
)


let private pageListItem model dispatch page =
  listItem [
    ListItemProp.Button true
    ListItemProp.Divider (page = Home)
    HTMLAttr.Selected (model.Page = page)
    Key (pageTitle page)
    DOMAttr.OnClick (fun _ -> Navigate page |> dispatch)
  ] [
    listItemText [ ] [ page |> pageTitle |> str ]
  ]

let private pageView model dispatch =
  match model.Page with
  | Home -> typography [] [ str "This app contains simple demos showing how certain Material-UI components can be used with Elmish." ]
  | AutoComplete -> lazyView2 AutoComplete.view model.AutoCompleteDownshift (AutoCompleteMsg >> dispatch)
  | Badges -> lazyView2 Badges.view model.Badges (BadgesMsg >> dispatch)
  | Dialogs -> lazyView2 Dialogs.view model.Dialogs (DialogsMsg >> dispatch)
  | SaveLoad -> lazyView2 SaveLoad.view model.SaveLoad (SaveLoadMsg >> dispatch)
  | Selects -> lazyView2 Selects.view model.Selects (SelectsMsg >> dispatch)
  | Snackbars -> lazyView2 Snackbars.view model.Snackbars (SnackbarsMsg >> dispatch)
  | StaticAssets ->
      div [] [
        typography [ Paragraph true ] [ str "This demo shows how to use static assets such as images." ]
        avatar [ Src (stat "avatar.jpg") ] []
      ]
  | TextFields -> lazyView2 TextFields.view model.TextFields (TextFieldsMsg >> dispatch)


let RootView = FunctionComponent.Of((fun(model, dispatch) ->
  let c = useStyles ()
  Html.div [
    prop.className c.root
    prop.children [
      Mui.cssBaseline []
      Mui.appBar [
        prop.className c.appBar
        prop.appBar.position.fixed'
        prop.children [
          Mui.toolbar [
            prop.children [
              Mui.typography [
                prop.typography.variant.h6
                prop.typography.color.inherit'
                prop.text (pageTitle model.Page)
              ]
            ]
          ]
        ]
      ]
      Mui.drawer [
        prop.className c.drawer
        prop.drawer.variant.permanent
        prop.drawer.classes [
          classes.drawer.paper c.drawerPaper
        ]
        prop.children [
          Html.div [ prop.className c.toolbar ]
          Mui.list [
            prop.list.component' "nav"
            prop.children (Page.All |> List.map (pageListItem model dispatch) |> ofList)
          ]
        ]
      ]
      main [ Class !!c.content ] [
        Html.div [ prop.className c.toolbar ]
        pageView model dispatch
      ]
    ]
  ]
), "RootView", memoEqualsButFunctions)

let view model dispatch =
  RootView (model, dispatch)
