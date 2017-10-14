///This file contains some dummy code to weave and look up to generated IL to test or fix ModuleWeaver
using Newtonsoft.Json;
using System;
using System.IO;
using TestFlask.Aspects;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Identifiers;
using TestFlask.Aspects.Player;

namespace AssemblyToProcess
{
    public class SomeResponse
    {
        public int SomeProperty { get; set; }

        public string SomeOtherProperty { get; set; }
    }

    public class SomeRequest
    {
        public int SomeReqProperty { get; set; }
    }

    public class SomeRequestIdentifier : IRequestIdentifier<SomeRequest>
    {
        public string ResolveDisplayInfo(SomeRequest req)
        {
            return $"SomeReqProperty:{req.SomeReqProperty}";
        }

        public string ResolveIdentifierKey(SomeRequest req)
        {
            return req.SomeReqProperty.ToString();
        }
    }

    public class GetFooArgsIdentifier : IRequestIdentifier<int, string, float>
    {
        public string ResolveDisplayInfo(int arg0, string arg1, float arg2)
        {
            throw new NotImplementedException();
        }

        public string ResolveIdentifierKey(int arg0, string arg1, float arg2)
        {
            throw new NotImplementedException();
        }
    }

    public class FooResponse
    {
        public int SomeProperty { get; set; }

        public string SomeOtherProperty { get; set; }
    }

    public class FooRequest
    {
        public int SomeReqProperty { get; set; }
    }


    public class SomeClient
    {

        //[Playback(typeof(SomeRequestIdentifier))]
        public SomeResponse GetSome(SomeRequest req)
        {
            var response = new SomeResponse
            {
                SomeProperty = req.SomeReqProperty,
                SomeOtherProperty = "someVal"
            };

            return response;
        }

        //[Playback]
        public FooResponse GetFoo(FooRequest req)
        {
            var response = new FooResponse
            {
                SomeProperty = req.SomeReqProperty,
                SomeOtherProperty = "someVal"
            };

            return response;
        }

        //[Playback]
        public SomeResponse ReturnSome()
        {
            return new SomeResponse
            {
                SomeProperty = 0,
                SomeOtherProperty = "no args"
            };
        }

        //[Playback]
        public void DoSome(SomeRequest req)
        {
            int a = 5 * 5;
            Console.WriteLine(a);
        }

        [Playback(typeof(GetFooArgsIdentifier))]
        public FooResponse GetFooWithTooManyArgs(int a, string str, float f)
        {
            var response = new FooResponse
            {
                SomeProperty = a,
                SomeOtherProperty = "someVal"
            };

            return response;
        }

        //[Playback]
        public void DoNoArgsNoResponse()
        {
            Console.WriteLine("Anooo");
        }

        #region IL Copy

        public SomeResponse GetSome_ExampleClone(SomeRequest req)
        {
            return null;
        }

        public void DoSome_ExampleClone(SomeRequest req)
        {
            int a = 5 * 5;
            Console.WriteLine(a);
        }

        public FooResponse GetFooWithTooManyArgs_ExampleClone(int a, string str, float f)
        {
            return null;
        }

        public void DoNoArgsNoResponse_ExampleClone()
        {
            Console.WriteLine("Anooo");
        }

        public SomeResponse RecorderWrapper_Example(SomeRequest req)
        {
            FuncPlayer<SomeRequest, SomeResponse> player = new FuncPlayer<SomeRequest, SomeResponse>("SomeResponse RecorderWrapper(SomeRequest)", new SomeRequestIdentifier(), null);

            player.StartInvocation(req);

            switch (player.DetermineTestMode(req))
            {
                case TestModes.NoMock:
                    return player.CallOriginal(req, GetSome_ExampleClone);
                case TestModes.Record:
                    return player.Record(req, GetSome_ExampleClone);
                case TestModes.Play:
                    return player.Play(req);
                default:
                    return null;
            }
        }

        public FooResponse RecorderWrapperWithArgs_Example(int i, string s, float f)
        {
            FuncPlayer<int, string, float, FooResponse> player = new FuncPlayer<int, string, float, FooResponse>("SomeResponse RecorderWrapper(SomeRequest)", new GetFooArgsIdentifier(), null);

            player.StartInvocation(i, s, f);

            switch (player.DetermineTestMode(i, s, f))
            {
                case TestModes.NoMock:
                    return player.CallOriginal(i, s, f, GetFooWithTooManyArgs_ExampleClone);
                case TestModes.Record:
                    return player.Record(i, s, f, GetFooWithTooManyArgs_ExampleClone);
                case TestModes.Play:
                    return player.Play(i, s, f);
                default:
                    return null;
            }
        }

        public void RecorderVoidWrapper_Example(SomeRequest req)
        {
            ActionPlayer<SomeRequest> player = new ActionPlayer<SomeRequest>("SomeResponse RecorderWrapper(SomeRequest)", new SomeRequestIdentifier());

            player.StartInvocation(req);

            switch (player.DetermineTestMode(req))
            {
                case TestModes.NoMock:
                    player.CallOriginal(req, DoSome_ExampleClone);
                    break;
                case TestModes.Record:
                    player.Record(req, DoSome_ExampleClone);
                    break;
                case TestModes.Play:
                    player.Play(req);
                    break;
                default:
                    break;
            }
        }

        public void DoNoArgsNoResponse_Example()
        {
            ActionPlayer playerVoid = new ActionPlayer("System.Void AssemblyToProcess.SomeClient::DoNoArgsNoResponse()", (IRequestIdentifier)null);
            playerVoid.StartInvocation();
            switch (playerVoid.DetermineTestMode())
            {
                case TestModes.NoMock:
                    playerVoid.CallOriginal(new Action(this.DoNoArgsNoResponse_ExampleClone));
                    break;
                case TestModes.Record:
                    playerVoid.Record(new Action(this.DoNoArgsNoResponse_ExampleClone));
                    break;
                case TestModes.Play:
                    playerVoid.Play();
                    break;
            }
        }

        #endregion

    }
}
