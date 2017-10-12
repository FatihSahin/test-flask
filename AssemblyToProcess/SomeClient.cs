
using Newtonsoft.Json;
using System;
using System.IO;
using TestFlask.Aspects;
using TestFlask.Aspects.Enums;
using TestFlask.Aspects.Identifiers;

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

        [Playback]
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

        //[Playback(typeof(GetFooArgsIdentifier))]
        public FooResponse GetFooWithTooManyArgs(int a, string str, float f)
        {
            var response = new FooResponse
            {
                SomeProperty = a,
                SomeOtherProperty = "someVal"
            };

            return response;
        }

        public SomeResponse GetSome_ExampleClone(SomeRequest req)
        {
            return null;
        }

        public FooResponse GetFooWithTooManyArgs_ExampleClone(int a, string str, float f)
        {
            return null;
        }

        public SomeResponse RecorderWrapper_Example(SomeRequest req)
        {
            Player<SomeRequest, SomeResponse> player = new Player<SomeRequest, SomeResponse>("SomeResponse RecorderWrapper(SomeRequest)", new SomeRequestIdentifier(), null);

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
            Player<int, string, float, FooResponse> player = new Player<int, string, float, FooResponse>("SomeResponse RecorderWrapper(SomeRequest)", new GetFooArgsIdentifier(), null);

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


    }
}
