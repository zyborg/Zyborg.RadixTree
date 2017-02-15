using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Zyborg.Collections
{
    [TestClass]
    public class RadixTreeTests
    {
		[TestMethod]
		public void TestFindLongestPrefix()
		{
			Assert.AreEqual(6, RadixTree<object>.FindLongestPrefix("abcdef", "abcdef"));
			Assert.AreEqual(3, RadixTree<object>.FindLongestPrefix("abcdef", "abcxyz"));
			Assert.AreEqual(3, RadixTree<object>.FindLongestPrefix("abc", "abcxyz"));
			Assert.AreEqual(3, RadixTree<object>.FindLongestPrefix("abcxyz", "abc"));
			Assert.AreEqual(0, RadixTree<object>.FindLongestPrefix("abc", "xyz"));
		}

		[TestMethod]
		public void TestValueTupleSwap()
		{
			var arr = new int[] { 10, 20, 30, 40 };

			(arr[0], arr[1]) = (arr[1], arr[0]);
			CollectionAssert.AreEqual(new int[] { 20, 10, 30, 40 }, arr);

			(arr[0], arr[1], arr[2], arr[3]) = (arr[3], arr[2], arr[1], arr[0]);
			CollectionAssert.AreEqual(new int[] { 40, 30, 10, 20 }, arr);
		}

		[TestMethod]
		//~ func TestRoot(t *testing.T) {
		public void TestRoot()
		{
			//~ r := New()
			var r = new RadixTree<int>();

			//~ _, ok := r.Delete("")
			//~ if ok {
			//~ 	t.Fatalf("bad")
			//~ }
			var (_, ok) = r.GoDelete("");
			Assert.IsFalse(ok, "Remove should fail");

			//~ _, ok = r.Insert("", true)
			//~ if ok {
			//~ 	t.Fatalf("bad")
			//~ }
			(_, ok) = r.GoInsert("", 0);
			Assert.IsFalse(ok, "Insert should not replace");

			//~ val, ok := r.Get("")
			//~ if !ok || val != true {
			//~ 	t.Fatalf("bad: %v", val)
			//~ }
			int val;
			(val, ok) = r.GoGet("");
			Assert.IsTrue(ok, "Get should not find key");
			Assert.AreEqual(default(int), val, "Get should return default int");

			//~ val, ok = r.Delete("")
			//~ if !ok || val != true {
			//~ 	t.Fatalf("bad: %v", val)
			//~ }
			(val, ok) = r.GoDelete("");
			Assert.IsTrue(ok, "Remove was successful");
			Assert.AreEqual(default(int), val, "Remove should return default int");
		}

		[TestMethod]
		//~ func TestDelete(t *testing.T) {
		public void TestDelete()
		{
			//~ r := New()
			var r = new RadixTree<int>();

			//~ s := []string{"", "A", "AB"}
			var s = new string[] { "", "A", "AB" };

			//~ for _, ss := range s {
			//~ 	r.Insert(ss, true)
			//~ }
			foreach (var ss in s)
			{
				r.GoInsert(ss, 1);
			}

			//~ for _, ss := range s {
			//~ 	_, ok := r.Delete(ss)
			//~ 	if !ok {
			//~ 		t.Fatalf("bad %q", ss)
			//~ 	}
			//~ }
			foreach (var ss in s)
			{
				var (_, ok) = r.GoDelete(ss);
				Assert.IsTrue(ok, "Removed key");
			}
		}

		[TestMethod]
		//~ func TestLongestPrefix(t *testing.T) {
		public void TestLongestPrefix()
		{
			//~ r := New()
			var r = new RadixTree<int>();

			//~ keys := []string{
			//~ 	"",
			//~ 	"foo",
			//~ 	"foobar",
			//~ 	"foobarbaz",
			//~ 	"foobarbazzip",
			//~ 	"foozip",
			//~ }
			var keys = new string[] {
				"",
				"foo",
				"foobar",
				"foobarbaz",
				"foobarbazzip",
				"foozip",
			};
			//~ for _, k := range keys {
			//~ 	r.Insert(k, nil)
			//~ }
			//~ if r.Len() != len(keys) {
			//~ 	t.Fatalf("bad len: %v %v", r.Len(), len(keys))
			//~ }
			foreach (var k in keys)
			{
				r.GoInsert(k, 1);
			}
			Assert.AreEqual(keys.Length, r.Count, "Tree count and key count match");

			//~ type exp struct {
			//~ 	inp string
			//~ 	out string
			//~ }
			//~ cases := []exp{
			//~ 	{"a", ""},
			//~ 	{"abc", ""},
			//~ 	{"fo", ""},
			//~ 	{"foo", "foo"},
			//~ 	{"foob", "foo"},
			//~ 	{"foobar", "foobar"},
			//~ 	{"foobarba", "foobar"},
			//~ 	{"foobarbaz", "foobarbaz"},
			//~ 	{"foobarbazzi", "foobarbaz"},
			//~ 	{"foobarbazzip", "foobarbazzip"},
			//~ 	{"foozi", "foo"},
			//~ 	{"foozip", "foozip"},
			//~ 	{"foozipzap", "foozip"},
			//~ }
			var cases = new (string inp, string @out)[]
			{
				("a", ""),
				("abc", ""),
				("fo", ""),
				("foo", "foo"),
				("foob", "foo"),
				("foobar", "foobar"),
				("foobarba", "foobar"),
				("foobarbaz", "foobarbaz"),
				("foobarbazzi", "foobarbaz"),
				("foobarbazzip", "foobarbazzip"),
				("foozi", "foo"),
				("foozip", "foozip"),
				("foozipzap", "foozip"),
			};
			//~ for _, test := range cases {
			//~ 	m, _, ok := r.LongestPrefix(test.inp)
			//~ 	if !ok {
			//~ 		t.Fatalf("no match: %v", test)
			//~ 	}
			//~ 	if m != test.out {
			//~ 		t.Fatalf("mis-match: %v %v", m, test)
			//~ 	}
			//~ }
			foreach (var test in cases)
			{
				var (m, _, ok) = r.LongestPrefix(test.inp);
				Assert.IsTrue(ok, "Found longest prefix match");
				Assert.AreEqual(test.@out, m, "Found expected longest prefix match");
			}
		}

		[TestMethod]
		//~ func TestWalkPrefix(t *testing.T) {
		public void TestWalkPrefix()
		{
			//~ r := New()
			var r = new RadixTree<int>();

			//~ keys := []string{
			//~ 	"foobar",
			//~ 	"foo/bar/baz",
			//~ 	"foo/baz/bar",
			//~ 	"foo/zip/zap",
			//~ 	"zipzap",
			//~ }
			var keys = new string[]
			{
				"foobar",
				"foo/bar/baz",
				"foo/baz/bar",
				"foo/zip/zap",
				"zipzap",
			};
			//~ for _, k := range keys {
			//~ 	r.Insert(k, nil)
			//~ }
			//~ if r.Len() != len(keys) {
			//~ 	t.Fatalf("bad len: %v %v", r.Len(), len(keys))
			//~ }
			foreach (var k in keys)
				r.GoInsert(k, 1);
			Assert.AreEqual(keys.Length, r.Count, "Tree count and key count match");

			//~ type exp struct {
			//~ 	inp string
			//~ 	out []string
			//~ }
			//~ cases := []exp{
			//~ 	{
			//~ 		"f",
			//~ 		[]string{"foobar", "foo/bar/baz", "foo/baz/bar", "foo/zip/zap"},
			//~ 	},
			//~ 	{
			//~ 		"foo",
			//~ 		[]string{"foobar", "foo/bar/baz", "foo/baz/bar", "foo/zip/zap"},
			//~ 	},
			//~ 	{
			//~ 		"foob",
			//~ 		[]string{"foobar"},
			//~ 	},
			//~ 	{
			//~ 		"foo/",
			//~ 		[]string{"foo/bar/baz", "foo/baz/bar", "foo/zip/zap"},
			//~ 	},
			//~ 	{
			//~ 		"foo/b",
			//~ 		[]string{"foo/bar/baz", "foo/baz/bar"},
			//~ 	},
			//~ 	{
			//~ 		"foo/ba",
			//~ 		[]string{"foo/bar/baz", "foo/baz/bar"},
			//~ 	},
			//~ 	{
			//~ 		"foo/bar",
			//~ 		[]string{"foo/bar/baz"},
			//~ 	},
			//~ 	{
			//~ 		"foo/bar/baz",
			//~ 		[]string{"foo/bar/baz"},
			//~ 	},
			//~ 	{
			//~ 		"foo/bar/bazoo",
			//~ 		[]string{},
			//~ 	},
			//~ 	{
			//~ 		"z",
			//~ 		[]string{"zipzap"},
			//~ 	},
			//~ }

			var cases = new (string inp, string[] @out)[] {
				(
					"f",
					new string[]{"foobar", "foo/bar/baz", "foo/baz/bar", "foo/zip/zap"}
				),(
					"foo",
					new string[]{"foobar", "foo/bar/baz", "foo/baz/bar", "foo/zip/zap"}
				),(
					"foob",
					new string[]{"foobar"}
				),(
					"foo/",
					new string[]{"foo/bar/baz", "foo/baz/bar", "foo/zip/zap"}
				),(
					"foo/b",
					new string[]{"foo/bar/baz", "foo/baz/bar"}
				),(
					"foo/ba",
					new string[]{"foo/bar/baz", "foo/baz/bar"}
				),(
					"foo/bar",
					new string[]{"foo/bar/baz"}
				),(
					"foo/bar/baz",
					new string[]{"foo/bar/baz"}
				),(
					"foo/bar/bazoo",
					new string[]{ }
				),(
					"z",
					new string[]{"zipzap"}
				),
			};

			//~ for _, test := range cases {
			//~ 	out := []string{}
			//~ 	fn := func(s string, v interface{}) bool {
			//~ 		out = append(out, s)
			//~ 		return false
			//~ 	}
			//~ 	r.WalkPrefix(test.inp, fn)
			//~ 	sort.Strings(out)
			//~ 	sort.Strings(test.out)
			//~ 	if !reflect.DeepEqual(out, test.out) {
			//~ 		t.Fatalf("mis-match: %v %v", out, test.out)
			//~ 	}
			//~ }
			foreach (var test in cases)
			{
				var @out = new List<string>();
				Walker<int> fn = (string s, int v) =>
				{
					@out.Add(s);
					return false;
				};
				r.WalkPrefix(test.inp, fn);
				@out.Sort();
				Array.Sort(test.@out);
				CollectionAssert.AreEqual(test.@out, @out.ToArray(), "Walked nodes are equal to expected");
			}
		}

		[TestMethod]
		//~ func TestWalkPath(t *testing.T) {
		public void TestWalkPath()
		{
			//~ r := New()
			var r = new RadixTree<int>();

			//~ keys := []string{
			//~ 	"foo",
			//~ 	"foo/bar",
			//~ 	"foo/bar/baz",
			//~ 	"foo/baz/bar",
			//~ 	"foo/zip/zap",
			//~ 	"zipzap",
			//~ }
			var keys = new string[]
			{
			   "foo",
			   "foo/bar",
			   "foo/bar/baz",
			   "foo/baz/bar",
			   "foo/zip/zap",
			   "zipzap",
			};
			//for _, k := range keys {
			//	r.Insert(k, nil)
			//}
			//if r.Len() != len(keys) {
			//	t.Fatalf("bad len: %v %v", r.Len(), len(keys))
			//}
			foreach (var k in keys)
				r.GoInsert(k, 1);
			Assert.AreEqual(keys.Length, r.Count, "Tree count is equal to key count");

			//~ type exp struct {
			//~ 	inp string
			//~ 	out []string
			//~ }
			//~ cases := []exp{
			//~ 	{
			//~ 		"f",
			//~ 		[]string{},
			//~ 	},
			//~ 	{
			//~ 		"foo",
			//~ 		[]string{"foo"},
			//~ 	},
			//~ 	{
			//~ 		"foo/",
			//~ 		[]string{"foo"},
			//~ 	},
			//~ 	{
			//~ 		"foo/ba",
			//~ 		[]string{"foo"},
			//~ 	},
			//~ 	{
			//~ 		"foo/bar",
			//~ 		[]string{"foo", "foo/bar"},
			//~ 	},
			//~ 	{
			//~ 		"foo/bar/baz",
			//~ 		[]string{"foo", "foo/bar", "foo/bar/baz"},
			//~ 	},
			//~ 	{
			//~ 		"foo/bar/bazoo",
			//~ 		[]string{"foo", "foo/bar", "foo/bar/baz"},
			//~ 	},
			//~ 	{
			//~ 		"z",
			//~ 		[]string{},
			//~ 	},
			//~ }
			var cases = new (string inp, string[] @out)[]
			{
				(
					"f",
					new string[] {}
				),(
					"foo",
					new string[] {"foo"}
				),(
					"foo/",
					new string[] {"foo"}
				),(
					"foo/ba",
					new string[] {"foo"}
				),(
					"foo/bar",
					new string[] {"foo", "foo/bar"}
				),(
					"foo/bar/baz",
					new string[] {"foo", "foo/bar", "foo/bar/baz"}
				),(
					"foo/bar/bazoo",
					new string[] {"foo", "foo/bar", "foo/bar/baz"}
				),(
					"z",
					new string[] { }
				)
			};

			//~ for _, test := range cases {
			//~ 	out := []string{}
			//~ 	fn := func(s string, v interface{}) bool {
			//~ 		out = append(out, s)
			//~ 		return false
			//~ 	}
			//~ 	r.WalkPath(test.inp, fn)
			//~ 	sort.Strings(out)
			//~ 	sort.Strings(test.out)
			//~ 	if !reflect.DeepEqual(out, test.out) {
			//~ 		t.Fatalf("mis-match: %v %v", out, test.out)
			//~ 	}
			//~ }
			foreach (var test in cases)
			{
				var @out = new List<string>();
				Walker<int> fn = (string s, int v) =>
				{
					@out.Add(s);
					return false;
				};
				r.WalkPath(test.inp, fn);
				@out.Sort();
				Array.Sort(test.@out);
				CollectionAssert.AreEqual(test.@out, @out.ToArray(), "Walked nodes are equal to expected");
			}
		}

		[TestMethod]
		//~ func TestRadix(t *testing.T) {
		public void TestRadix()
		{
			//~ var min, max string
			//~ inp := make(map[string]interface{})
			//~ for i := 0; i < 1000; i++ {
			//~ 	gen := generateUUID()
			//~ 	inp[gen] = i
			//~ 	if gen < min || i == 0 {
			//~ 		min = gen
			//~ 	}
			//~ 	if gen > max || i == 0 {
			//~ 		max = gen
			//~ 	}
			//~ }
			var min = string.Empty;
			var max = string.Empty;
			var inp = new Dictionary<string, int>();
			for (int i = 0; i < 1000; i++)
			{
				var gen = GenerateUUID();
				inp[gen] = i;
				if (i == 0 || string.CompareOrdinal(gen, min) < 0)
					min = gen;
				if (i == 0 || string.CompareOrdinal(gen, max) > 0)
					max = gen;
			}

			//~ r := NewFromMap(inp)
			//~ if r.Len() != len(inp) {
			//~ 	t.Fatalf("bad length: %v %v", r.Len(), len(inp))
			//~ }
			var r = new RadixTree<int>(inp);
			Assert.AreEqual(inp.Count, r.Count, "Tree count matches input map count");


			//~ r.Walk(func(k string, v interface{}) bool {
			//~ 	println(k)
			//~ 	return false
			//~ })
			r.Walk((k, v) =>
			{
				Console.WriteLine(k);
				return false;
			});

			//~ for k, v := range inp {
			//~ 	out, ok := r.Get(k)
			//~ 	if !ok {
			//~ 		t.Fatalf("missing key: %v", k)
			//~ 	}
			//~ 	if out != v {
			//~ 		t.Fatalf("value mis-match: %v %v", out, v)
			//~ 	}
			//~ }
			foreach (var kv in inp)
			{
				var (@out, ok) = r.GoGet(kv.Key);
				Assert.IsTrue(ok, "Contains key for Get");
				Assert.AreEqual(kv.Value, @out, "Found expected value by key");
			}

			// Check min and max
			//~ outMin, _, _ := r.Minimum()
			//~ if outMin != min {
			//~ 	t.Fatalf("bad minimum: %v %v", outMin, min)
			//~ }
			//~ outMax, _, _ := r.Maximum()
			//~ if outMax != max {
			//~ 	t.Fatalf("bad maximum: %v %v", outMax, max)
			//~ }
			var (outMin, _, _) = r.Minimum();
			Assert.AreEqual(min, outMin, "Expected minimum");
			var (outMax, _, _) = r.Maximum();
			Assert.AreEqual(max, outMax, "Expected maximum");

			//~ for k, v := range inp {
			//~ 	out, ok := r.Delete(k)
			//~ 	if !ok {
			//~ 		t.Fatalf("missing key: %v", k)
			//~ 	}
			//~ 	if out != v {
			//~ 		t.Fatalf("value mis-match: %v %v", out, v)
			//~ 	}
			//~ }
			foreach (var kv in inp)
			{
				var (@out, ok) = r.GoDelete(kv.Key);
				Assert.IsTrue(ok, "Contains key for Remove");
				Assert.AreEqual(kv.Value, @out, "Removed expected value by key");
			}

			//~ if r.Len() != 0 {
			//~ 	t.Fatalf("bad length: %v", r.Len())
			//~ }
			Assert.AreEqual(0, r.Count, "Empty tree (after deleting all)");
		}


		[TestMethod]
		//~ func TestRadix(t *testing.T) {
		public void TestRadixSmall()
		{
			Action<RadixTree<int>> printer = x =>
			{
				Debug.WriteLine("----------------------------");
				using (var ms = new MemoryStream())
				{
					x.Print(ms);
					Debug.WriteLine(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
				}
				Debug.WriteLine("----------------------------");
			};

			//~ var min, max string
			//~ inp := make(map[string]interface{})
			//~ for i := 0; i < 1000; i++ {
			//~ 	gen := generateUUID()
			//~ 	inp[gen] = i
			//~ 	if gen < min || i == 0 {
			//~ 		min = gen
			//~ 	}
			//~ 	if gen > max || i == 0 {
			//~ 		max = gen
			//~ 	}
			//~ }
			var min = char.MaxValue.ToString();
			var max = string.Empty;
			var inp = new Dictionary<string, int>()
			{
				["f"] = 1,
				["ab"] = 1,
				["foo"] = 1,
				["bar"] = 1,
				["a"] = 1,
				["foobar"] = 1,
				["abc"] = 1,
			};
			foreach (var gen in inp.Keys)
			{
				if (string.CompareOrdinal(gen, min) < 0)
					min = gen;
				if (string.CompareOrdinal(gen, max) > 0)
					max = gen;
			}

			//~ r := NewFromMap(inp)
			//~ if r.Len() != len(inp) {
			//~ 	t.Fatalf("bad length: %v %v", r.Len(), len(inp))
			//~ }
			var r = new RadixTree<int>();
			printer(r);
			foreach (var kv in inp)
			{
				r.GoInsert(kv.Key, kv.Value);
				printer(r);
			}

			Assert.AreEqual(inp.Count, r.Count, "Tree count matches input map count");


			//~ r.Walk(func(k string, v interface{}) bool {
			//~ 	println(k)
			//~ 	return false
			//~ })
			r.Walk((k, v) =>
			{
				Debug.WriteLine(k);
				return false;
			});


			//~ for k, v := range inp {
			//~ 	out, ok := r.Get(k)
			//~ 	if !ok {
			//~ 		t.Fatalf("missing key: %v", k)
			//~ 	}
			//~ 	if out != v {
			//~ 		t.Fatalf("value mis-match: %v %v", out, v)
			//~ 	}
			//~ }
			foreach (var kv in inp)
			{
				var (@out, ok) = r.GoGet(kv.Key);
				Assert.IsTrue(ok, "Contains key for Get");
				Assert.AreEqual(kv.Value, @out, "Found expected value by key");
			}

			// Check min and max
			//~ outMin, _, _ := r.Minimum()
			//~ if outMin != min {
			//~ 	t.Fatalf("bad minimum: %v %v", outMin, min)
			//~ }
			//~ outMax, _, _ := r.Maximum()
			//~ if outMax != max {
			//~ 	t.Fatalf("bad maximum: %v %v", outMax, max)
			//~ }
			var (outMin, _, _) = r.Minimum();
			Assert.AreEqual(min, outMin, "Expected minimum");
			var (outMax, _, _) = r.Maximum();
			Assert.AreEqual(max, outMax, "Expected maximum");

			//~ for k, v := range inp {
			//~ 	out, ok := r.Delete(k)
			//~ 	if !ok {
			//~ 		t.Fatalf("missing key: %v", k)
			//~ 	}
			//~ 	if out != v {
			//~ 		t.Fatalf("value mis-match: %v %v", out, v)
			//~ 	}
			//~ }
			foreach (var kv in inp)
			{
				var (@out, ok) = r.GoDelete(kv.Key);
				Assert.IsTrue(ok, "Contains key for Remove");
				Assert.AreEqual(kv.Value, @out, "Removed expected value by key");
			}

			//~ if r.Len() != 0 {
			//~ 	t.Fatalf("bad length: %v", r.Len())
			//~ }
			Assert.AreEqual(0, r.Count, "Empty tree (after deleting all)");
		}

		// generateUUID is used to generate a random UUID
		//~ func generateUUID() string {
		public string GenerateUUID()
		{
			//~ buf := make([]byte, 16)
			//~ if _, err := crand.Read(buf); err != nil {
			//~ 	panic(fmt.Errorf("failed to read random bytes: %v", err))
			//~ }
			//~ return fmt.Sprintf("%08x-%04x-%04x-%04x-%12x",
			//~ 	buf[0:4],
			//~ 	buf[4:6],
			//~ 	buf[6:8],
			//~ 	buf[8:10],
			//~ 	buf[10:16])
			return Guid.NewGuid().ToString();
		}
    }
}
